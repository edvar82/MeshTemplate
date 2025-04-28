using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;

public class SpatialMeshLoader : MonoBehaviour
{
    [SerializeField]
    private string meshObjectPath = "SpatialMeshes/SpatialMapping_04_marker_to_right"; // Caminho sem "Assets/Resources/" e sem extensão

    [SerializeField]
    public Material spatialMeshMaterial; // Material para a malha espacial

    [SerializeField]
    private bool displayMesh = true; // Controla a visibilidade da malha

    [SerializeField]
    private bool enableColliders = true; // Controla se os colliders estão habilitados

    private GameObject spatialMeshObject; // Referência ao objeto de malha carregado

    void Start()
    {
        // Verificar quais recursos estão disponíveis
        Object[] assets = Resources.LoadAll("SpatialMeshes");
        Debug.Log($"Encontrados {assets.Length} recursos na pasta SpatialMeshes:");

        foreach (Object asset in assets)
        {
            Debug.Log($"- {asset.name} (tipo: {asset.GetType().Name})");
        }

        // Desabilitar o observador automático de spatial mesh (se estiver usando apenas dados carregados)
        DisableRealTimeMeshObserver();

        // Carregar o mesh do arquivo
        LoadMeshFromFile();
    }

    void DisableRealTimeMeshObserver()
    {
        // Correção para MRTK 2.8
        var spatialAwarenessSystem = CoreServices.SpatialAwarenessSystem;
        if (spatialAwarenessSystem != null)
        {
            // Desabilitar observadores para não conflitar com o mesh carregado
            spatialAwarenessSystem.SuspendObservers();
            Debug.Log("Observadores de malha espacial do MRTK suspensos");
        }
    }

    private void AddSafetyFloor()
    {
        // Criar um plano grande como chão de segurança
        GameObject safetyFloor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        safetyFloor.name = "SafetyFloor";

        // Tornar o plano 10x maior que o padrão
        safetyFloor.transform.localScale = new Vector3(10f, 1f, 10f);

        // Posicionar abaixo da câmera/personagem
        safetyFloor.transform.position = new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y - 1.5f, // Posicionar abaixo dos pés
            Camera.main.transform.position.z);

        // Tornar o chão invisível mas com colisão
        Renderer renderer = safetyFloor.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false; // Invisível
        }

        // Fazer o chão filho do objeto de malha espacial
        safetyFloor.transform.parent = spatialMeshObject.transform;

        Debug.Log("Chão de segurança adicionado sob o personagem");
    }

    void LoadMeshFromFile()
    {
        Debug.Log($"Tentando carregar mesh do caminho: {meshObjectPath}");

        // Carregar como GameObject (prefab)
        GameObject modelPrefab = Resources.Load<GameObject>(meshObjectPath);

        if (modelPrefab != null)
        {
            // Se carregou como GameObject, instancie e use
            spatialMeshObject = Instantiate(modelPrefab);
            spatialMeshObject.name = "Loaded Spatial Mesh";
            Debug.Log($"Mesh carregado como prefab GameObject: {modelPrefab.name}");

            // Configurar material e colliders
            ConfigureMeshProperties(spatialMeshObject);

            // Garantir visibilidade e posicionamento adequado
            spatialMeshObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
            Debug.Log($"Mesh posicionado em: {spatialMeshObject.transform.position}");
        }
        else
        {
            // Tentar carregar como Mesh direto
            Mesh meshAsset = Resources.Load<Mesh>(meshObjectPath);

            if (meshAsset != null)
            {
                // Se carregou como Mesh, crie um GameObject para ele
                spatialMeshObject = new GameObject("Loaded Spatial Mesh");

                MeshFilter meshFilter = spatialMeshObject.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = meshAsset;

                MeshRenderer meshRenderer = spatialMeshObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = spatialMeshMaterial != null ?
                    spatialMeshMaterial : new Material(Shader.Find("Standard"));

                // Adicionar collider
                MeshCollider collider = spatialMeshObject.AddComponent<MeshCollider>();
                collider.sharedMesh = meshAsset;
                collider.enabled = enableColliders;

                // Posicionar
                spatialMeshObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2;
                Debug.Log($"Mesh carregado como asset de Mesh e posicionado em: {spatialMeshObject.transform.position}");
            }
            else
            {
                Debug.LogError($"Não foi possível carregar o mesh espacial do caminho: {meshObjectPath}");
            }
        }
        AddSafetyFloor();
    }

    void ConfigureMeshProperties(GameObject meshObj)
    {
        // Obter todos os MeshRenderer no objeto e seus filhos
        MeshRenderer[] renderers = meshObj.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            // Configurar visibilidade
            renderer.enabled = displayMesh;

            // Aplicar material se fornecido
            if (spatialMeshMaterial != null)
            {
                renderer.material = spatialMeshMaterial;
            }
        }

        // Configurar colliders
        MeshCollider[] existingColliders = meshObj.GetComponentsInChildren<MeshCollider>();

        // Se não houver colliders ou estiverem todos desabilitados, adicione novos
        if (existingColliders.Length == 0)
        {
            Debug.Log("Nenhum collider encontrado, adicionando novos colliders");
            MeshFilter[] meshFilters = meshObj.GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter filter in meshFilters)
            {
                MeshCollider collider = filter.gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = filter.sharedMesh;
                collider.enabled = enableColliders;
                collider.convex = false; // Para malhas complexas, deve ser false
                collider.isTrigger = false; // Importante para colisões físicas
                Debug.Log($"Adicionado collider ao objeto: {filter.gameObject.name}");
            }
        }
        else
        {
            // Configurar colliders existentes
            foreach (MeshCollider collider in existingColliders)
            {
                collider.enabled = enableColliders;
                collider.isTrigger = false; // Importante para colisões físicas
                Debug.Log($"Configurado collider existente: {collider.gameObject.name}, habilitado: {collider.enabled}");
            }
        }

        // Garantir que todos os objetos têm a layer correta
        SetLayerRecursively(meshObj, LayerMask.NameToLayer("Default"));
    }

    // Método auxiliar para configurar a layer em todos os objetos filhos
    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    // Método adicional para acessar o objeto do mesh espacial de outros scripts
    public GameObject GetSpatialMeshObject()
    {
        return spatialMeshObject;
    }

    // Este método pode ser chamado de fora para alterar a visibilidade da malha
    public void SetMeshVisibility(bool isVisible)
    {
        displayMesh = isVisible;

        if (spatialMeshObject != null)
        {
            MeshRenderer[] renderers = spatialMeshObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.enabled = displayMesh;
            }
        }
    }

    // Este método pode ser chamado para alterar o material da malha
    public void SetMeshMaterial(Material newMaterial)
    {
        if (newMaterial != null && spatialMeshObject != null)
        {
            spatialMeshMaterial = newMaterial;

            MeshRenderer[] renderers = spatialMeshObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.material = spatialMeshMaterial;
            }
        }
    }
}