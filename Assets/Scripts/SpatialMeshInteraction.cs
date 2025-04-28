using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class SpatialMeshInteraction : MonoBehaviour, IMixedRealityPointerHandler
{
    private SpatialMeshLoader meshLoader;
    [SerializeField] private Material highlightMaterial;
    private Material defaultMaterial;
    private GameObject lastHitObject;

    // Flag para debug
    [SerializeField] private bool debugMode = true;

    void Start()
    {
        // Encontrar o SpatialMeshLoader
        meshLoader = FindObjectOfType<SpatialMeshLoader>();
        if (meshLoader != null)
        {
            defaultMaterial = meshLoader.spatialMeshMaterial;
            Debug.Log($"SpatialMeshLoader encontrado, material padrão: {(defaultMaterial != null ? defaultMaterial.name : "null")}");
        }
        else
        {
            Debug.LogError("SpatialMeshLoader não encontrado na cena!");
        }

        // Registrar explicitamente para eventos de input
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);

        if (debugMode)
            Debug.Log("SpatialMeshInteraction iniciado e registrado para eventos de pointer");
    }

    void OnDestroy()
    {
        // Importante: desregistrar quando o objeto for destruído
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        if (debugMode)
            Debug.Log("OnPointerDown detectado");

        // Verificar se o raycast atingiu algo
        if (eventData.Pointer.Result == null)
        {
            if (debugMode) Debug.Log("Pointer Result é nulo");
            return;
        }

        GameObject hitObject = eventData.Pointer.Result.CurrentPointerTarget;
        if (hitObject == null)
        {
            if (debugMode) Debug.Log("Não acertou nenhum objeto");
            return;
        }

        if (debugMode)
            Debug.Log($"Hit em objeto: {hitObject.name}, layer: {LayerMask.LayerToName(hitObject.layer)}");

        // Verificar referência do meshLoader e seu objeto
        if (meshLoader == null || meshLoader.GetSpatialMeshObject() == null)
        {
            Debug.LogWarning("MeshLoader ou SpatialMeshObject é nulo!");
            return;
        }

        // Verificar se o objeto atingido é parte da malha espacial
        if (hitObject.transform.IsChildOf(meshLoader.GetSpatialMeshObject().transform))
        {
            Debug.Log("Tocou na malha espacial: " + hitObject.name);

            // Restaurar material anterior
            if (lastHitObject != null && lastHitObject != hitObject)
            {
                Renderer lastRenderer = lastHitObject.GetComponent<Renderer>();
                if (lastRenderer != null && defaultMaterial != null)
                {
                    lastRenderer.material = defaultMaterial;
                }
            }

            lastHitObject = hitObject;

            // Aplicar material de destaque
            Renderer currentRenderer = hitObject.GetComponent<Renderer>();
            if (currentRenderer != null && highlightMaterial != null)
            {
                currentRenderer.material = highlightMaterial;
                Debug.Log($"Material de destaque aplicado a: {hitObject.name}");
            }
            else
            {
                Debug.LogWarning($"Renderer ou material de destaque é nulo para o objeto: {hitObject.name}");
            }
        }
        else
        {
            if (debugMode)
                Debug.Log($"Objeto {hitObject.name} não é parte da malha espacial");
        }
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (debugMode) Debug.Log("OnPointerClicked");
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }
}