using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;

public class AdvancedSpatialMeshLoader : MonoBehaviour
{
    [SerializeField] private string meshObjectPath = "SpatialMeshes/SpatialMapping_04_marker_to_right";
    [SerializeField] public Material spatialMeshMaterial;

    private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem;
    private GameObject spatialMeshObject;

    void Start()
    {
        // Obter o sistema de SpatialAwareness
        spatialAwarenessSystem = CoreServices.SpatialAwarenessSystem;
        if (spatialAwarenessSystem == null)
        {
            Debug.LogError("O sistema SpatialAwareness não está disponível");
            return;
        }

        // Carregar o mesh
        LoadMeshAndRegisterWithSpatialAwarenessSystem();
    }

    void LoadMeshAndRegisterWithSpatialAwarenessSystem()
    {
        // Carregar o mesh do Resources
        Mesh loadedMesh = Resources.Load<Mesh>(meshObjectPath);
        if (loadedMesh == null)
        {
            GameObject modelPrefab = Resources.Load<GameObject>(meshObjectPath);
            if (modelPrefab == null)
            {
                Debug.LogError($"Não foi possível carregar o mesh: {meshObjectPath}");
                return;
            }

            // Se carregou como GameObject, extrai o mesh dele
            MeshFilter meshFilter = modelPrefab.GetComponentInChildren<MeshFilter>();
            if (meshFilter != null)
                loadedMesh = meshFilter.sharedMesh;
        }

        if (loadedMesh == null)
        {
            Debug.LogError("Não foi possível obter o mesh");
            return;
        }

        // Criar um ID único para o mesh
        int meshId = loadedMesh.GetInstanceID();

        // Obter os observadores de mesh usando o método atualizado
        IMixedRealityDataProviderAccess dataProviderAccess = spatialAwarenessSystem as IMixedRealityDataProviderAccess;
        if (dataProviderAccess == null)
        {
            Debug.LogError("Sistema de SpatialAwareness não implementa IMixedRealityDataProviderAccess");
            return;
        }

        IReadOnlyList<IMixedRealitySpatialAwarenessMeshObserver> meshObservers =
            dataProviderAccess.GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();

        if (meshObservers.Count == 0)
        {
            Debug.LogError("Não há observadores de mesh disponíveis");
            return;
        }

        // Obter o primeiro observador disponível
        IMixedRealitySpatialAwarenessMeshObserver observer = meshObservers[0];

        try
        {
            // Suspender observadores para não interferir com nosso mesh manual
            spatialAwarenessSystem.SuspendObservers();

            // Criar um GameObject para o mesh
            spatialMeshObject = new GameObject($"SpatialMesh_{meshId}");

            // Adicionar os componentes necessários
            MeshFilter meshFilter = spatialMeshObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = loadedMesh;

            MeshRenderer meshRenderer = spatialMeshObject.AddComponent<MeshRenderer>();

            // Usar o material fornecido em vez de tentar acessar o material do observer
            meshRenderer.sharedMaterial = spatialMeshMaterial != null ?
                spatialMeshMaterial : new Material(Shader.Find("Standard"));

            MeshCollider meshCollider = spatialMeshObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = loadedMesh;

            // Configurar a layer corretamente
            spatialMeshObject.layer = observer.MeshPhysicsLayer;

            Debug.Log($"Mesh carregado e configurado como SpatialMesh ID: {meshId}");

            // Posicionar o mesh em um local visível
            spatialMeshObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;

            AddSafetyFloor();
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao criar SpatialMesh: {e.Message}");
        }
    }

    // Adicione este método à classe AdvancedSpatialMeshLoader

    private void AddSafetyFloor()
    {
        // Criar um plano grande como chão de segurança
        GameObject safetyFloor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        safetyFloor.name = "SafetyFloor";

        // Tornar o plano 10x maior que o padrão (cobrindo uma área grande)
        safetyFloor.transform.localScale = new Vector3(10f, 1f, 10f);

        // Posicionar abaixo da câmera/personagem
        float floorHeight = Camera.main.transform.position.y - 1.8f; // 1.8m é altura típica de uma pessoa
        safetyFloor.transform.position = new Vector3(
            Camera.main.transform.position.x,
            floorHeight,
            Camera.main.transform.position.z);

        // Tornar o chão invisível mas mantendo colisão
        Renderer renderer = safetyFloor.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false; // Invisível
        }

        // Garantir que o chão esteja na layer correta para colisão
        safetyFloor.layer = LayerMask.NameToLayer("Default");

        // Adicionar como filho do objeto de malha espacial para organização
        if (spatialMeshObject != null)
        {
            safetyFloor.transform.parent = spatialMeshObject.transform;
        }

        Debug.Log("Chão de segurança invisível adicionado em Y = " + floorHeight);
    }

    // Método para acessar o objeto de malha espacial de outros scripts
    public GameObject GetSpatialMeshObject()
    {
        return spatialMeshObject;
    }
}