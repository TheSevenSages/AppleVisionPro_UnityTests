using System.Collections;
using UnityEngine;
using DataStructures;

/// <summary>
/// Attatch to the "Main Camera" object inside of the XR origin to send the headset's transform data over the server.
/// </summary>
public class HeadsetTelemetry : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The interval (in ms) in which this script will send telemetry data.")]
    private int sendLatency_ms = 250;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("Sender");
    }

    private IEnumerator Sender()
    {
        while (true)
        {
            Debug.Log("sent");
            TelemetryPayload payload = new TelemetryPayload(gameObject.transform, "HEADSET");
            Messenger.SendTelemetry(payload);
            //yield return new WaitForSeconds(sendLatency_ms / 1000.0f);
            yield return new WaitForFixedUpdate();
        }
    }
}
