using HoloToolkit.Unity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

namespace DataStructures
{
    // Represents another client connected to the server
    [Serializable]
    public class Device
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    [Serializable]
    public class TransformPayload
    {
        public TransformPayload() { }
        public TransformPayload(Transform t)
        {
            posX = t.position.x;
            posY = t.position.y;
            posZ = t.position.z;

            rotX = t.rotation.x;
            rotY = t.rotation.y;
            rotZ = t.rotation.z;
            rotW = t.rotation.w;
        }
        public TransformPayload(Vector3 t)
        {
            posX = t.x;
            posY = t.y;
            posZ = t.z;
        }

        public float posX = 0.0f;
        public float posY = 0.0f;
        public float posZ = 0.0f;
        public float rotX = 0.0f;
        public float rotY = 0.0f;
        public float rotZ = 0.0f;
        public float rotW = 0.0f;
    }

    [Serializable]
    // Mesh messages
    public class MeshPayload : TransformPayload
    {
        public enum MeshTypes
        {
            UNKNOWN,
            WALL,
            FLOOR
        }

        public MeshTypes type = MeshTypes.UNKNOWN;
        public string encodedMesh = "";

        public MeshPayload(Transform t) : base(t) {}
        public MeshPayload(Mesh m)
        {
            encodedMesh = EncodeMesh(m);
        }
        public MeshPayload(Transform t, Mesh m) : base(t) 
        {
            encodedMesh = EncodeMesh(m);
        }

        public static string EncodeMesh(Mesh mesh)
        {
            byte[] meshBytes = SimpleMeshSerializer.Serialize(mesh);
            string encoded = Convert.ToBase64String(meshBytes);
            return encoded;
        }

        public static Mesh DecodeMesh(string encoded)
        {
            byte[] meshBytes = Convert.FromBase64String(encoded);
            Mesh m = SimpleMeshSerializer.DeserializeSingleMesh(meshBytes);
            return m;
        }
    }

    [Serializable]
    public class MeshesPayload
    {
        public bool foundFloor = false;
        public float floorY = 0.0f;
        public List<MeshPayload> meshes = new List<MeshPayload>();
    }
}
