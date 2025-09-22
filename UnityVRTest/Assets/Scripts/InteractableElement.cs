using Unity.PolySpatial;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.VisionOS;

public class InteractableElement : MonoBehaviour
{
    // Lifecycle functions for the interactable object
    [SerializeField] private UnityEvent<InteractionData> OnClick;
    [SerializeField] private UnityEvent<InteractionData> OnHold;
    [SerializeField] private UnityEvent<InteractionData> OnRelease;

    private UserInput input;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = UserInput.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        if (input.IsInteractionStart(gameObject))
        {
            GetComponent<VisionOSHoverEffect>().enabled = false;
            OnClick.Invoke(input.GetInteractionData());
        }
        else if (input.IsInteractionOngoing(gameObject))
        {
            OnHold.Invoke(input.GetInteractionData());
        }
        else if (input.IsInteractionEnd(gameObject))
        {
            OnRelease.Invoke(input.GetInteractionData());
            GetComponent<VisionOSHoverEffect>().enabled = true;
        }
    }
}
