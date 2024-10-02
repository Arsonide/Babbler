using UnityEngine;

namespace Babbler.Implementation.Occlusion.Vents;

public class VentTagInteractable : MonoBehaviour
{
    private void Start()
    {
        Transform t = transform;

        Vector3 transformPosition = t.position;
        Vector3 centerPosition = transformPosition;
        Vector3 ventPosition = transformPosition + t.forward * 0.425f - t.up * 0.375f;

        VentRegistry.QueueVentTagRegistration(new VentTagCache
        {
            TransformPosition = transformPosition,
            CenterPosition = centerPosition,
            AudioPosition = ventPosition,
        });
    }
}