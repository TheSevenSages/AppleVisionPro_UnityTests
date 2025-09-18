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
public class UserInput : MonoBehaviour
{
    // Singleton pattern
    private static UserInput instance = null;

    private long selected_ID = -1;
    private enum InteractState { NONE, BEGIN, ONGOING, END };
    private InteractState i_state;

    private void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
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
    private SpatialPointerPhase GetInteractionPhase()
    {
        return InputSystem.actions.FindAction("Select").ReadValue<SpatialPointerState>().phase;
    }
    // Returns true if the given object is in the "Began" phase of interaction
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
