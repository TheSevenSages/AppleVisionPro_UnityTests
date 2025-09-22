using UnityEngine;
using UnityEngine.InputSystem;
using Unity.PolySpatial.XR;
using Unity.PolySpatial.InputDevices;
using UnityEngine.XR.VisionOS.InputDevices;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Unity.Networking.Transport.Error;
using System.Security.Cryptography;
using SpatialPointerPhase = UnityEngine.InputSystem.LowLevel.SpatialPointerPhase;

// Data structure used to pass information on to interaction events
public class InteractionData
{
    public Vector3 Position; // The position of the interaction in world space
    public Vector3 DeltaPosition; // The change in the position between the prior interaction and this one

    public Vector3 HandPosition; // The position of the user's pinch (between the user's thumb and index finger).
    public Quaternion HandRotation; // The rotation of the user's pinch (between the user's thumb and index finger).

    public Vector3 HeadPosition; // The position of the user's HMD in worldspace
    public Quaternion HeadRotation; // The rotation of the user's HMD

    public SpatialPointerKind Kind; // The interaction kind, Touch (poke), In Direct Pinch, Direct Pinch, Pointer, Stylus.
};
public class UserInput : MonoBehaviour
{
    // Singleton pattern
    private static UserInput instance = null;

    private long selected_ID = -1;
    private enum InteractState { NONE, BEGIN, ONGOING, END };
    private InteractState i_state;

    private InteractionData data;

    private void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            data = new InteractionData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Updates the interaction state
        if (i_state == InteractState.NONE)
        {
            if (GetInteractionPhase() == SpatialPointerPhase.Began)
            {
                selected_ID = InputSystem.actions.FindAction("Select").ReadValue<SpatialPointerState>().targetId;
                i_state = InteractState.BEGIN;
            }
        }
        else if (i_state == InteractState.BEGIN)
        {
            i_state = InteractState.ONGOING;
        }
        else if (i_state == InteractState.ONGOING)
        {
            if (GetInteractionPhase() == SpatialPointerPhase.Ended || GetInteractionPhase() == SpatialPointerPhase.Cancelled)
            {
                i_state = InteractState.END;
            }
        }
        else if (i_state == InteractState.END)
        {
            i_state = InteractState.NONE;
        }
    }

    private void LateUpdate()
    {
        
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public static UserInput GetInstance()
    {
        return instance;
    }

    // User positioning
    public Vector3 GetHeadPosition()
    {
        return InputSystem.actions.FindAction("HeadsetPosition").ReadValue<Vector3>();
    }

    public Quaternion GetHeadRotation()
    {
        return InputSystem.actions.FindAction("HeadsetRotation").ReadValue<Quaternion>();
    }

    // Interactions
    public GameObject GetLookedAtObject()
    {
        GameObject target = InputSystem.actions.FindAction("Select").ReadValue<SpatialPointerState>().targetObject;
        return target;
    }
    public Vector3 GetInteractPosition()
    {
        return InputSystem.actions.FindAction("Select").ReadValue<SpatialPointerState>().interactionPosition;
    }
    private SpatialPointerPhase GetInteractionPhase()
    {
        return InputSystem.actions.FindAction("Select").ReadValue<SpatialPointerState>().phase;
    }
    public InteractionData GetInteractionData()
    {
        data.Position = GetInteractPosition();
        data.DeltaPosition = InputSystem.actions.FindAction("Select").ReadValue<SpatialPointerState>().deltaInteractionPosition;

        data.HandPosition = InputSystem.actions.FindAction("Select").ReadValue<SpatialPointerState>().inputDevicePosition;
        data.HandRotation = InputSystem.actions.FindAction("Select").ReadValue<SpatialPointerState>().inputDeviceRotation;

        data.HeadPosition = GetHeadPosition();
        data.HeadRotation = GetHeadRotation();

        data.Kind = InputSystem.actions.FindAction("Select").ReadValue<SpatialPointerState>().Kind;

        return data;
    }
    public bool IsInteractionStart(GameObject obj)
    {
        if (i_state != InteractState.BEGIN) { return false; } // if not in correct state
        if (selected_ID != obj.GetInstanceID()) { return false; } // if interacted object is not the one given
        return true;
    }
    public bool IsInteractionOngoing(GameObject obj)
    {
        if (i_state != InteractState.ONGOING) { return false; } // if not in correct state
        if (selected_ID != obj.GetInstanceID()) { return false; } // if interacted object is not the one given
        return true;
    }
    public bool IsInteractionEnd(GameObject obj)
    {
        if (i_state != InteractState.END) { return false; } // if not in correct state
        if (selected_ID != obj.GetInstanceID()) { return false; } // if interacted object is not the one given
        return true;
    }
}
