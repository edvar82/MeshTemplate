using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;

public class SpatialMeshInteraction : MonoBehaviour, IMixedRealityPointerHandler
{
    [SerializeField] private Material highlightMaterial;
    private Material defaultMaterial;
    private GameObject lastHitObject;
    private AdvancedSpatialMeshLoader meshLoader;

    void Start()
    {
        meshLoader = FindObjectOfType<AdvancedSpatialMeshLoader>();
        if (meshLoader != null)
        {
            defaultMaterial = meshLoader.spatialMeshMaterial;
            Debug.Log("Obtido material do AdvancedSpatialMeshLoader");
        }
        else
        {
            // Se não encontrar o loader, tenta obter o material padrão do MRTK
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = CoreServices.SpatialAwarenessSystem;
            if (spatialAwarenessSystem != null)
            {
                // Cast para a interface correta para acessar GetDataProviders<T>
                IMixedRealityDataProviderAccess dataProviderAccess = spatialAwarenessSystem as IMixedRealityDataProviderAccess;
                if (dataProviderAccess != null)
                {
                    IReadOnlyList<IMixedRealitySpatialAwarenessMeshObserver> observers =
                        dataProviderAccess.GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();

                    if (observers.Count > 0)
                    {
                        // Como não há meshMaterial, usamos um material padrão
                        defaultMaterial = new Material(Shader.Find("Standard"));
                        Debug.Log("Criado material padrão para interação com a malha");
                    }
                }
            }
        }

        // Se o material ainda for nulo, criar um material padrão
        if (defaultMaterial == null)
        {
            defaultMaterial = new Material(Shader.Find("Standard"));
            defaultMaterial.color = Color.gray;
        }

        // Registrar para eventos de input
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
        Debug.Log("SpatialMeshInteraction iniciado");
    }

    void OnDestroy()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        if (eventData.Pointer.Result != null)
        {
            GameObject hitObject = eventData.Pointer.Result.CurrentPointerTarget;

            if (hitObject != null)
            {
                // Verificar se o objeto está na layer de Spatial Awareness
                if (hitObject.layer == LayerMask.NameToLayer("Spatial Awareness"))
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
                    Renderer renderer = hitObject.GetComponent<Renderer>();
                    if (renderer != null && highlightMaterial != null)
                    {
                        renderer.material = highlightMaterial;
                    }
                }
            }
        }
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }
    public void OnPointerUp(MixedRealityPointerEventData eventData) { }
}