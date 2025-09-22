using UnityEngine;

public class TestButtonFunctionality : MonoBehaviour
{
    private Material og_mat;
    [SerializeField] private Material a_mat;
    [SerializeField] private Material b_mat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        og_mat = GetComponent<Renderer>().material;
    }

    public void Click(InteractionData data)
    {
        GetComponent<Renderer>().material = a_mat;
    }

    public void Hold(InteractionData data)
    {
        GetComponent<Renderer>().material = b_mat;
    }

    public void Release(InteractionData data)
    {
        GetComponent<Renderer>().material = og_mat;
    }
}
