using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;

public class SpatialMeshLoader : MonoBehaviour
{
    [SerializeField] private GameObject spatialMeshPrefab;
    [SerializeField] public Material spatialMeshMaterial;

    [Header("Configura��es de Posicionamento")]
    [SerializeField] private float distanciaDaCamera = 2f; // Qu�o longe o objeto ficar� da c�mera
    [SerializeField] private Vector3 offset = new Vector3(40f, -0.5f, 0); // Ajuste fino de posi��o

    private GameObject spatialMeshInstance;
    private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem;
    private bool useLiveMesh = false;

    void Start()
    {
        // Obter refer�ncia ao sistema de Spatial Awareness do MRTK
        spatialAwarenessSystem = CoreServices.SpatialAwarenessSystem;

        if (useLiveMesh)
        {
            // Configurar para usar o mapeamento espacial em tempo real
            ConfigureLiveMeshObserver();
        }
        else
        {
            // Carregar o modelo OBJ pr�-mapeado
            LoadSpatialMeshFromPrefab();
        }
    }

    private void ConfigureLiveMeshObserver()
    {
        // Verificar se o sistema est� dispon�vel
        if (spatialAwarenessSystem != null)
        {
            // Usar GetDataProviders em vez de GetObservers (que est� obsoleto)
            var dataProviderAccess = spatialAwarenessSystem as IMixedRealityDataProviderAccess;
            if (dataProviderAccess != null)
            {
                IReadOnlyList<IMixedRealitySpatialAwarenessMeshObserver> observers =
                    dataProviderAccess.GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();

                if (observers.Count > 0)
                {
                    // Configurar o observador com par�metros personalizados
                    var observer = observers[0];
                    observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Visible;

                    // Configurar o material diretamente no observador
                    if (observer is BaseSpatialMeshObserver meshObserver)
                    {
                        meshObserver.VisibleMaterial = spatialMeshMaterial;
                    }

                    // Resumir a observa��o se foi suspensa
                    if (!observer.IsRunning)
                    {
                        observer.Resume();
                    }

                    Debug.Log("Iniciando observa��o de malhas espaciais em tempo real");
                }
                else
                {
                    Debug.LogError("Nenhum observador de malha espacial encontrado");
                }
            }
            else
            {
                Debug.LogError("Sistema de Spatial Awareness n�o implementa IMixedRealityDataProviderAccess");
            }
        }
        else
        {
            Debug.LogError("Sistema de Spatial Awareness n�o encontrado");
        }
    }

    private void LoadSpatialMeshFromPrefab()
    {
        if (spatialMeshPrefab != null)
        {
            // Obter a c�mera principal
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("C�mera principal n�o encontrada, usando posi��o padr�o");
                spatialMeshInstance = Instantiate(spatialMeshPrefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                // Calcular posi��o � frente da c�mera
                Vector3 cameraPosition = mainCamera.transform.position;
                Vector3 cameraForward = mainCamera.transform.forward;

                // Posicionar o objeto na dire��o do olhar da c�mera
                Vector3 objectPosition = cameraPosition + (cameraForward * distanciaDaCamera) + offset;

                // Instanciar o objeto na posi��o calculada
                spatialMeshInstance = Instantiate(spatialMeshPrefab, objectPosition, Quaternion.identity);

                Debug.Log("Objeto posicionado na dire��o do olhar da c�mera: " + objectPosition);
            }

            // Configurar material da malha
            MeshRenderer[] renderers = spatialMeshInstance.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                if (spatialMeshMaterial != null)
                {
                    renderer.material = spatialMeshMaterial;
                }
            }

            // Adicionar colliders se n�o existirem
            MeshFilter[] filters = spatialMeshInstance.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter filter in filters)
            {
                if (filter.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider collider = filter.gameObject.AddComponent<MeshCollider>();
                    collider.sharedMesh = filter.sharedMesh;
                }
            }

            Debug.Log("Malha espacial carregada do prefab");
        }
        else
        {
            Debug.LogError("Prefab da malha espacial n�o atribu�do");
        }
    }

    // M�todo para alternar entre malha pr�-mapeada e em tempo real
    public void ToggleMeshMode()
    {
        useLiveMesh = !useLiveMesh;

        if (spatialMeshInstance != null)
        {
            Destroy(spatialMeshInstance);
        }

        if (spatialAwarenessSystem != null)
        {
            // Suspender os observadores
            var dataProviderAccess = spatialAwarenessSystem as IMixedRealityDataProviderAccess;
            if (dataProviderAccess != null)
            {
                var observers = dataProviderAccess.GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();
                foreach (var observer in observers)
                {
                    observer.Suspend();
                }
            }
        }

        if (useLiveMesh)
        {
            ConfigureLiveMeshObserver();
        }
        else
        {
            LoadSpatialMeshFromPrefab();
        }
    }

    // M�todos para interagir com a malha espacial
    public GameObject GetSpatialMeshObject()
    {
        return spatialMeshInstance;
    }

    public void SetMeshMaterial(Material material)
    {
        spatialMeshMaterial = material;

        if (spatialMeshInstance != null)
        {
            MeshRenderer[] renderers = spatialMeshInstance.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.material = material;
            }
        }

        // Atualiza tamb�m o material no observador de malha, se estiver usando malha ao vivo
        if (useLiveMesh && spatialAwarenessSystem != null)
        {
            var dataProviderAccess = spatialAwarenessSystem as IMixedRealityDataProviderAccess;
            if (dataProviderAccess != null)
            {
                var observers = dataProviderAccess.GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();

                foreach (var observer in observers)
                {
                    if (observer is BaseSpatialMeshObserver meshObserver)
                    {
                        meshObserver.VisibleMaterial = material;
                    }
                }
            }
        }
    }
}