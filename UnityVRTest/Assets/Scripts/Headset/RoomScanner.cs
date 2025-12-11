using DataStructures;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RoomScanner : MonoBehaviour
{
    public float sendLatency_ms = 1000;

    private Dictionary<TrackableId, ARPlane> allPlanes = new Dictionary<TrackableId, ARPlane>();

    private TrackableId? floorId = null;
    private TrackableId? ceilingId = null;
    private float floorY = Mathf.Infinity;
    private float ceilingY = Mathf.NegativeInfinity;

    private ARPlaneManager planeManager;

    private void Awake()
    {
        planeManager = gameObject.GetComponent<ARPlaneManager>();
        planeManager.enabled = false;
    }

    public void StartScanning()
    {
        planeManager.enabled = true;
        StartCoroutine("SendAllPlanes");
    }

    public void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> changes)
    {
        foreach (var plane in changes.added)
        {
            // Handle added planes
            allPlanes[plane.trackableId] = plane;
            Debug.Log("Plane added!");
        }

        foreach (var plane in changes.updated)
        {
            // Handle updated planes
            allPlanes[plane.trackableId] = plane;
            Debug.Log("Plane updated!");
        }

        foreach (var pair in changes.removed)
        {
            // Handle removed planes
            pair.Value.gameObject.GetComponent<MeshFilter>().mesh = new Mesh();
            if (floorId == pair.Key) { floorId = null; floorY = Mathf.Infinity; }
            else if (ceilingId == pair.Key) { ceilingId = null; ceilingY = Mathf.NegativeInfinity; }

            allPlanes[pair.Key] = pair.Value;
            Debug.Log("Plane removed!");
        }
    }

    private IEnumerator SendAllPlanes()
    {
        while (true)
        {
            MeshesPayload _meshes = new MeshesPayload();

            foreach (var plane in allPlanes.Values)
            {
                // If plane is subsumed we can basically delete its mesh
                if (plane.subsumedBy != null) { plane.gameObject.GetComponent<MeshFilter>().mesh = new Mesh(); }

                MeshPayload _mesh = new MeshPayload(plane.gameObject.transform, plane.gameObject.GetComponent<MeshFilter>().mesh);
                _mesh.meshId = plane.trackableId.GetHashCode();
                // Determine if the plane is a wall, floor, ceiling, or object and set the mesh type accordingly
                Quaternion meshRotation = new Quaternion(_mesh.rotX, _mesh.rotY, _mesh.rotZ, _mesh.rotW);
                if (Math.Abs(meshRotation.eulerAngles.x) > 45/* && plane.size.y >= Mathf.Abs(ceilingY - floorY) * 0.8f*/)
                {
                    _mesh.type = MeshPayload.MeshTypes.WALL;
                }
                else if (_mesh.posY <= floorY || plane.trackableId == floorId)
                {
                    floorId = plane.trackableId;
                    floorY = _mesh.posY;
                    _meshes.foundFloor = true;
                    _meshes.floorY = floorY;
                    _mesh.type = MeshPayload.MeshTypes.FLOOR;
                }
                else if (_mesh.posY >= ceilingY || plane.trackableId == ceilingId)
                {
                    ceilingId = plane.trackableId;
                    ceilingY = _mesh.posY;
                    _mesh.type = MeshPayload.MeshTypes.CEILING;
                }
                else
                {
                    _mesh.type = MeshPayload.MeshTypes.OBJECT;
                }

                _meshes.meshes.Add(_mesh);
            }

            if (allPlanes.Count > 0)
            {
                Messenger.SendMeshes(_meshes);
                allPlanes.Clear();
            }

            yield return new WaitForSeconds(sendLatency_ms / 1000.0f);
        }
    }
}
