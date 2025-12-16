using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Networking.Transport.Error;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.ReloadAttribute;
using DataStructures;

public static class Messenger
{
    // All of the available message types
    public enum MessageTypes
    {
        TEXT,
        MESHES,
        TELEMETRY
    }
    // Represents a message sent from the server
    public class Message
    {
        public MessageTypes type;
        public string message;
    }

    [HideInInspector]
    public static UnityEvent<string> TextMessageEvent = new UnityEvent<string>();
    [HideInInspector]
    public static UnityEvent<MeshesPayload> MeshesMessageEvent = new UnityEvent<MeshesPayload>();
    [HideInInspector]
    public static UnityEvent<TelemetryPayload> TelemetryMessageEvent = new UnityEvent<TelemetryPayload>();

    /// <summary>
    /// Processes a message from the server according to its type.
    /// </summary>
    /// <param name="m">The message to process.</param>
    public static void ProcessIncomingMessage(Message m)
    {
        try
        {
            switch (m.type)
            {
                case MessageTypes.TEXT:
                    TextMessageEvent.Invoke(m.message);
                    break;

                case MessageTypes.MESHES:
                    var meshesPayload = JsonConvert.DeserializeObject<MeshesPayload>(m.message);
                    MeshesMessageEvent.Invoke(meshesPayload);
                    break;

                case MessageTypes.TELEMETRY:
                    var telemetryPayload = JsonConvert.DeserializeObject<TelemetryPayload>(m.message);
                    TelemetryMessageEvent.Invoke(telemetryPayload);
                    break;

                default:
                    Debug.LogWarning($"Recieved package of unknown type: {m.type}");
                    break;
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError($"Failed to process incoming message of type {m.type}: " + e.Message);
        }
    }

    // TODO: ABSTRACT THE MESSAGE PAYLOAD CLASS INTO ONLY THIS SCRIPT
    public static void SendMeshes(MeshesPayload meshes)
    {
        Message m = new Message();
        m.type = MessageTypes.MESHES;
        m.message = JsonConvert.SerializeObject(meshes);
        Backend.SendMessage(m);
    }

    public static void SendTelemetry(TelemetryPayload telemetry)
    {
        Message m = new Message();
        m.type = MessageTypes.TELEMETRY;
        m.message = JsonConvert.SerializeObject(telemetry);
        Backend.SendMessage(m);
    }
}
