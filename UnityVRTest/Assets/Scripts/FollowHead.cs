using UnityEngine;

public class FollowHead : MonoBehaviour
{
    UserInput input;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = UserInput.GetInstance();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = input.GetHeadPosition();
        transform.rotation = input.GetHeadRotation();
    }
}
