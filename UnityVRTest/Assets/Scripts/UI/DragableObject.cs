using UnityEngine;

public class DragableObject : MonoBehaviour
{
    public GameObject dragObject; // Object to be moved
    public void Drag(InteractionData data)
    {
        dragObject.transform.position += data.DeltaPosition;

        Vector3 lookat_pos = data.HeadPosition;
        lookat_pos.y = dragObject.transform.position.y;
        dragObject.transform.LookAt(lookat_pos);
        dragObject.transform.Rotate(new Vector3(0, 90, 90));
    }
}
