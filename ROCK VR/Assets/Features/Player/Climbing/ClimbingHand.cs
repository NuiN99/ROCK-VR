using System.Collections;
using System.Collections.Generic;
using NuiN.NExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbingHand : MonoBehaviour
{
    [SerializeField] InputActionReference grabAction;
    
    [SerializeField] Joint joint;
    [SerializeField] Rigidbody playerRB;

    [SerializeField, ReadOnly] bool _touching;
    [SerializeField, ReadOnly] bool _grabbing;

    Vector3 _grabPosition;

    void OnTriggerEnter(Collider other)
    {
        _touching = true;
    }

    void OnTriggerExit(Collider other)
    {
        _touching = false;
    }

    void Update()
    {
        if (_touching && grabAction.action.WasPressedThisFrame())
        {
            StartGrab();
        }
        else if (grabAction.action.WasReleasedThisFrame())
        {
            Release();
        }

        if (_grabbing)
        {
            Hold();
        }
    }

    void StartGrab()
    {
        _grabPosition = transform.position;
        joint.connectedBody = playerRB;
        
        _grabbing = true;
    }

    void Hold()
    {
        joint.anchor = playerRB.transform.TransformPoint(_grabPosition);
        joint.connectedAnchor = playerRB.transform.position;
        transform.position = _grabPosition;
    }

    void Release()
    {
        joint.connectedBody = null;
        _grabbing = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(playerRB.transform.TransformPoint(_grabPosition), 0.2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(playerRB.transform.position, 0.2f);
        
        Gizmos.color = Color.white;
    }
}
