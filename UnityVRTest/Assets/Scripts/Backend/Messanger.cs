using Unity.Networking.Transport.Error;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.ReloadAttribute;

public static class Messanger
{
    // All of the available message types
    public enum MessageTypes
    {
        TEXT,
        MESHES
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
    public static UnityEvent<string> MeshMessageEvent = new UnityEvent<string>();

    public static void ProcessIncomingMessage(Message m)
    {
        switch (m.type)
        {
            case MessageTypes.TEXT:
                TextMessageEvent.Invoke(m.message);
                break;

            case MessageTypes.MESHES:

                break;

            default:
                Debug.LogWarning($"Recieved package of unknown type: {m.type}");
                break;
        }
    }
}
