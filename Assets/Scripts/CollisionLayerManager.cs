using UnityEngine;
using Microsoft.MixedReality.Toolkit;

public class CollisionLayerManager : MonoBehaviour
{
    void Start()
    {
        // For�ar colis�o entre Spatial (layer 8) e Default (layer 0)
        Physics.IgnoreLayerCollision(8, 0, false); // false = n�o ignorar (permitir colis�o)
        Debug.Log("Colis�o entre Spatial e Default habilitada via c�digo");
    }

    // Para garantir que as configura��es persistam
    void FixedUpdate()
    {
        // Refor�ar a cada poucos segundos, pois o MRTK pode sobrescrever
        if (Time.frameCount % 100 == 0)
        {
            Physics.IgnoreLayerCollision(8, 0, false);
        }
    }
}