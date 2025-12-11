using DataStructures;
using UnityEngine;

public class TestSendMesh : MonoBehaviour
{
    public void SendMesh()
    {
        MeshPayload mesh = new MeshPayload(gameObject.GetComponent<MeshFilter>().mesh);

        MeshesPayload meshes = new MeshesPayload();
        meshes.meshes.Add(mesh);
        Messenger.SendMeshes(meshes);
    }
}
