using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class SpatialMeshInteraction : MonoBehaviour, IMixedRealityPointerHandler
{
    private SpatialMeshLoader meshLoader;
    [SerializeField] private Material highlightMaterial;
    private Material defaultMaterial;
    private GameObject lastHitObject;

    void Start()
    {
        meshLoader = FindObjectOfType<SpatialMeshLoader>();
        if (meshLoader != null)
        {
            defaultMaterial = meshLoader.GetComponent<SpatialMeshLoader>().spatialMeshMaterial;
        }
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        // Verificar se o raycast atingiu a malha espacial
        if (eventData.Pointer.Result != null)
        {
            GameObject hitObject = eventData.Pointer.Result.CurrentPointerTarget;

            if (hitObject != null && hitObject.transform.IsChildOf(meshLoader.GetSpatialMeshObject().transform))
            {
                Debug.Log("Tocou na malha espacial: " + hitObject.name);

                // Destacar o objeto atingido
                if (lastHitObject != null && lastHitObject != hitObject)
                {
                    lastHitObject.GetComponent<Renderer>().material = defaultMaterial;
                }

                lastHitObject = hitObject;

                // Aplicar material de destaque
                if (highlightMaterial != null)
                {
                    hitObject.GetComponent<Renderer>().material = highlightMaterial;
                }

                // Aqui você pode implementar outras interações com a malha
            }
        }
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }
    public void OnPointerUp(MixedRealityPointerEventData eventData) { }
}