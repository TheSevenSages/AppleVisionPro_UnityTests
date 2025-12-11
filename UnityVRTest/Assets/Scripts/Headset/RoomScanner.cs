using DataStructures;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RoomScanner : MonoBehaviour
{
    public float sendLatency_ms = 500;

    private Dictionary<TrackableId, ARPlane> allPlanes = new Dictionary<TrackableId, ARPlane>();

    private void Start()
    {
        StartCoroutine("SendAllPlanes");
    }

    public void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARPlane> changes)
    {
        foreach (var plane in changes.added)
        {
            // Handle added planes
            allPlanes.Add(plane.trackableId, plane);
            Debug.Log("Plane added!");
        }

        foreach (var plane in changes.updated)
        {
            // Handle updated planes
            //allPlanes[plane.trackableId] = plane;
            Debug.Log("Plane updated!");
        }

        foreach (var pair in changes.removed)
        {
            // Handle removed planes
            allPlanes.Remove(pair.Key);
            Debug.Log("Plane removed!");
        }
    }

    private IEnumerator SendAllPlanes()
    {
        while (true)
        {
            Debug.Log("Send");
            MeshesPayload _meshes = new MeshesPayload();

            foreach (var plane in allPlanes.Values)
            {
                MeshPayload _mesh = new MeshPayload(plane.gameObject.transform, plane.gameObject.GetComponent<MeshFilter>().mesh);

                // Determine if the plane is a wall or floor and set the payload type accordingly

                _meshes.meshes.Add(_mesh);
            }

            Messenger.SendMeshes(_meshes);

            yield return new WaitForSeconds(sendLatency_ms / 1000.0f);
        }
    }
}
