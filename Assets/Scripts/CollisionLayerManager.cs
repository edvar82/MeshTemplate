using UnityEngine;
using Microsoft.MixedReality.Toolkit;

public class CollisionLayerManager : MonoBehaviour
{
    void Start()
    {
        // Forçar colisão entre Spatial (layer 8) e Default (layer 0)
        Physics.IgnoreLayerCollision(8, 0, false); // false = não ignorar (permitir colisão)
        Debug.Log("Colisão entre Spatial e Default habilitada via código");
    }

    // Para garantir que as configurações persistam
    void FixedUpdate()
    {
        // Reforçar a cada poucos segundos, pois o MRTK pode sobrescrever
        if (Time.frameCount % 100 == 0)
        {
            Physics.IgnoreLayerCollision(8, 0, false);
        }
    }
}