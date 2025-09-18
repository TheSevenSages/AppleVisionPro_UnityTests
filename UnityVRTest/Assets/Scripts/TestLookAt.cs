using UnityEngine;
using UnityEngine.InputSystem;
public class TestLookAt : MonoBehaviour
{
    [SerializeField]
    private GameObject p_Head;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(p_Head.transform);
    }
}
