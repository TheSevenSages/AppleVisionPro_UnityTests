using UnityEngine;

public class DataStructures
{
    // Represents another client connected to the server
    [System.Serializable]
    public class Device
    {
        public string name { get; set; }
        public string id { get; set; }
    }
}
