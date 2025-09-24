using UnityEngine;

public class MoverRenderer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Material m_Material;
    void Start()
    {
        m_Material = GetComponent<Renderer>().material;
    }

    public void OnClick(InteractionData data)
    {
        m_Material.SetFloat("_IsHeld", 1);
    }
    public void OnRelease(InteractionData data)
    {
        m_Material.SetFloat("_IsHeld", 0);
    }
}
