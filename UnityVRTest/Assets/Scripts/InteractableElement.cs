using Unity.PolySpatial;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.VisionOS;
public class InteractableElement : MonoBehaviour
{
    private Material og_mat;
    [SerializeField] private Material a_mat;
    [SerializeField] private Material b_mat;

    // Lifecycle functions for the interactable object
    [SerializeField] private UnityEvent OnClickStart;
    [SerializeField] private UnityEvent OnClick;
    [SerializeField] private UnityEvent OnClickEnd;

    private UserInput input;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = UserInput.GetInstance();
        og_mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (input.IsInteractionStart(gameObject))
        {
            GetComponent<Renderer>().material = a_mat;
            GetComponent<VisionOSHoverEffect>().enabled = false;
        }
        else if (input.IsInteractionOngoing(gameObject))
        {
            GetComponent<Renderer>().material = b_mat;
        }
        else if (input.IsInteractionEnd(gameObject))
        {
            GetComponent<Renderer>().material = og_mat;
            GetComponent<VisionOSHoverEffect>().enabled = true;
        }
    }
}
