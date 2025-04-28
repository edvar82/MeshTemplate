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
            Debug.LogError("O sistema SpatialAwareness n�o est� dispon�vel");
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
                Debug.LogError($"N�o foi poss�vel carregar o mesh: {meshObjectPath}");
                return;
            }

            // Se carregou como GameObject, extrai o mesh dele
            MeshFilter meshFilter = modelPrefab.GetComponentInChildren<MeshFilter>();
            if (meshFilter != null)
                loadedMesh = meshFilter.sharedMesh;
        }

        if (loadedMesh == null)
        {
            Debug.LogError("N�o foi poss�vel obter o mesh");
            return;
        }

        // Criar um ID �nico para o mesh
        int meshId = loadedMesh.GetInstanceID();

        // Obter os observadores de mesh usando o m�todo atualizado
        IMixedRealityDataProviderAccess dataProviderAccess = spatialAwarenessSystem as IMixedRealityDataProviderAccess;
        if (dataProviderAccess == null)
        {
            Debug.LogError("Sistema de SpatialAwareness n�o implementa IMixedRealityDataProviderAccess");
            return;
        }

        IReadOnlyList<IMixedRealitySpatialAwarenessMeshObserver> meshObservers =
            dataProviderAccess.GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();

        if (meshObservers.Count == 0)
        {
            Debug.LogError("N�o h� observadores de mesh dispon�veis");
            return;
        }

        // Obter o primeiro observador dispon�vel
        IMixedRealitySpatialAwarenessMeshObserver observer = meshObservers[0];

        try
        {
            // Suspender observadores para n�o interferir com nosso mesh manual
            spatialAwarenessSystem.SuspendObservers();

            // Criar um GameObject para o mesh
            spatialMeshObject = new GameObject($"SpatialMesh_{meshId}");

            // Adicionar os componentes necess�rios
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

            // Posicionar o mesh em um local vis�vel
            spatialMeshObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;

            AddSafetyFloor();
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao criar SpatialMesh: {e.Message}");
        }
    }

    // Adicione este m�todo � classe AdvancedSpatialMeshLoader

    private void AddSafetyFloor()
    {
        // Criar um plano grande como ch�o de seguran�a
        GameObject safetyFloor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        safetyFloor.name = "SafetyFloor";

        // Tornar o plano 10x maior que o padr�o (cobrindo uma �rea grande)
        safetyFloor.transform.localScale = new Vector3(10f, 1f, 10f);

        // Posicionar abaixo da c�mera/personagem
        float floorHeight = Camera.main.transform.position.y - 1.8f; // 1.8m � altura t�pica de uma pessoa
        safetyFloor.transform.position = new Vector3(
            Camera.main.transform.position.x,
            floorHeight,
            Camera.main.transform.position.z);

        // Tornar o ch�o invis�vel mas mantendo colis�o
        Renderer renderer = safetyFloor.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false; // Invis�vel
        }

        // Garantir que o ch�o esteja na layer correta para colis�o
        safetyFloor.layer = LayerMask.NameToLayer("Default");

        // Adicionar como filho do objeto de malha espacial para organiza��o
        if (spatialMeshObject != null)
        {
            safetyFloor.transform.parent = spatialMeshObject.transform;
        }

        Debug.Log("Ch�o de seguran�a invis�vel adicionado em Y = " + floorHeight);
    }

    // M�todo para acessar o objeto de malha espacial de outros scripts
    public GameObject GetSpatialMeshObject()
    {
        return spatialMeshObject;
    }
}