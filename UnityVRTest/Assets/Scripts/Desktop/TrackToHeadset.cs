using UnityEngine;
using DataStructures;

/// <summary>
/// Causes the gameobject this script is attatched to to follow the position and rotation of the headset in worldspace.
/// </summary>
public class TrackToHeadset : MonoBehaviour
{
    private Transform trackingTransform;
    private void Awake()
    {
        Messenger.TelemetryMessageEvent.AddListener(GetTelemetry);
        trackingTransform = GetComponent<Transform>();
    }

    private void GetTelemetry(TelemetryPayload payload)
    {
        if (payload.tag != "HEADSET") { return; }

        trackingTransform.position = new Vector3(payload.posX, payload.posY, payload.posZ);
        trackingTransform.rotation = new Quaternion(payload.rotX, payload.rotY, payload.rotZ, payload.rotW);
    }

    private void OnDestroy()
    {
        Messenger.TelemetryMessageEvent.RemoveListener(GetTelemetry);
    }
}
