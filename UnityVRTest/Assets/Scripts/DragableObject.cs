using UnityEngine;

public class DragableObject : MonoBehaviour
{
    public GameObject dragObject; // Object to be moved
    public void Drag(InteractionData data)
    {
        dragObject.transform.position += data.DeltaPosition;
        dragObject.transform.LookAt(data.HeadPosition);
        dragObject.transform.Rotate(new Vector3(0, 90, 90));
    }
}
