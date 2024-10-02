using UnityEngine;

namespace Babbler.Implementation.Occlusion.Vents;

public class VentTagPeekable : MonoBehaviour
{
    // Peek ducts are immediately destroyed upon creation as they are mesh combined. On Awake they are not in the right spot, and Start will never be called.
    private void OnDestroy()
    {
        Transform t = transform;

        Vector3 transformPosition = t.position;
        Vector3 centerPosition = transformPosition + t.forward * 0.425f;
        Vector3 ventPosition = centerPosition - t.up * 0.375f;
        
        VentRegistry.QueueVentTagRegistration(new VentTagCache
        {
            TransformPosition = transformPosition,
            CenterPosition = centerPosition,
            AudioPosition = ventPosition,
        });
    }
}