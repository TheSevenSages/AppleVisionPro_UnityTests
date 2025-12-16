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
    Dictionary<int, GameObject> roomObjects = new Dictionary<int, GameObject>();

    // Materials
    [Header("RoomScan Materials")]
    public Material floorMat;
    public Material wallMat;
    public Material objMat;
    public Material ceilMat;

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
                    GameObject obj;
                    if (roomObjects.ContainsKey(mesh.meshId))
                    {
                        obj = roomObjects[mesh.meshId];
                    }
                    else
                    {
                        obj = new GameObject();
                        obj.AddComponent<MeshFilter>();
                        obj.AddComponent<MeshRenderer>();
                        roomObjects[mesh.meshId] = obj;
                    }

                    obj.transform.position = new Vector3(mesh.posX, mesh.posY, mesh.posZ);
                    obj.transform.rotation = new Quaternion(mesh.rotX, mesh.rotY, mesh.rotZ, mesh.rotW);
                    obj.transform.parent = transform;

                    var filter = obj.GetComponent<MeshFilter>();
                    filter.mesh = MeshPayload.DecodeMesh(mesh.encodedMesh);

                    // Render the mesh
                    var renderer = obj.GetComponent<MeshRenderer>();
                    switch (mesh.type)
                    {
                        case MeshPayload.MeshTypes.WALL:
                            renderer.material = wallMat;
                            break;

                        case MeshPayload.MeshTypes.OBJECT:
                            renderer.material = objMat;
                            break;

                        case MeshPayload.MeshTypes.FLOOR:
                            renderer.material = floorMat;
                            break;

                        case MeshPayload.MeshTypes.CEILING:
                            renderer.material = ceilMat;
                            break;

                        default:
                            // Not part of the room, do not render here
                            roomObjects.Remove(mesh.meshId);
                            Destroy(obj);
                            break;
                    }
                }
            }
            payloads.Clear();
        }
        catch (Exception e) 
        {
            Debug.LogError(e);
        }
    }

    public void ClearRoomObjects()
    {
        foreach (var pair in roomObjects)
        {
            Destroy(pair.Value);
        }
        roomObjects.Clear();
    }

    private void GetMeshes(MeshesPayload payload)
    {
        payloads.Add(payload);
    }

    private void OnDestroy()
    {
        Messenger.MeshesMessageEvent.RemoveListener(GetMeshes);
    }
}
