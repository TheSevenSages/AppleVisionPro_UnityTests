using DataStructures;
using HoloToolkit.Unity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomViewer : MonoBehaviour
{
    List<MeshesPayload> payloads = new List<MeshesPayload>();

    // Materials
    public Material def;
    void Awake()
    {
        Messenger.MeshesMessageEvent.AddListener(GetMeshes);
    }

    private void Update()
    {
        try
        {
            foreach (var payload in payloads)
            {
                foreach (var mesh in payload.meshes)
                {

                    var obj = new GameObject();
                    obj.transform.position = new Vector3(mesh.posX, mesh.posY, mesh.posZ);
                    obj.transform.rotation = new Quaternion(mesh.rotX, mesh.rotY, mesh.rotZ, mesh.rotW);
                    obj.transform.parent = transform;

                    var filter = obj.AddComponent<MeshFilter>();
                    filter.mesh = MeshPayload.DecodeMesh(mesh.encodedMesh);

                    // Render the mesh
                    var renderer = obj.AddComponent<MeshRenderer>();
                    renderer.material = def;
                }
            }
            payloads.Clear();
        }
        catch (Exception e) 
        {
            Debug.LogError(e);
        }
    }

    private void GetMeshes(MeshesPayload payload)
    {
        payloads.Add(payload);
    }
}
