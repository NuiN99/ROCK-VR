using System.Collections;
using System.Collections.Generic;
using NuiN.NExtensions;
using UnityEngine;

public class PlayerSwinging : MonoBehaviour
{
    Rigidbody _anchor;
    bool _addedRigidbody;
    bool _attached;

    [SerializeField] float maxAttachDistance = 25f;
    
    [SerializeField] KeyCode activateKey = KeyCode.Mouse1;
    [SerializeField] Transform head;
    [SerializeField] LayerMask attachableLayers;

    [SerializeField] Rigidbody rb;
    [SerializeField] ConfigurableJoint joint;

    Vector3 ConnectedAnchorWorld => _anchor.transform.TransformPoint(joint.connectedAnchor);

    void Update()
    {
        if (Input.GetKeyDown(activateKey)) Activate();
        else if(Input.GetKeyUp(activateKey)) Detach();

        if (!_attached)
            joint.connectedAnchor = transform.position;
    }

    void Activate()
    {
        if (!Physics.Raycast(head.position, head.forward, out RaycastHit hit, maxAttachDistance, attachableLayers)) return;
        if (!hit.collider.TryGetComponent(out _anchor))
        {
            _anchor = hit.collider.gameObject.AddComponent<Rigidbody>();
            _anchor.isKinematic = true;
            _addedRigidbody = true;
        }
        else
        {
            _addedRigidbody = false;
        }

        SoftJointLimit limit = joint.linearLimit;
        limit.limit = Vector3.Distance(transform.position, hit.point);
        joint.linearLimit = limit;
            
        joint.connectedBody = _anchor;
        joint.connectedAnchor = _anchor.gameObject.transform.InverseTransformPoint(hit.point);

        _attached = true;
    }

    void Detach()
    {
        joint.connectedBody = null;
        if(_anchor != null && _addedRigidbody) Destroy(_anchor);

        _attached = false;
    }

    void OnDrawGizmos()
    {
        if (!_attached)
        {
            if (!Physics.Raycast(head.position, head.forward, out RaycastHit hit, maxAttachDistance, attachableLayers)) return;
            Gizmos.DrawSphere(hit.point, 0.25f);
            return;
        }
        Debug.DrawLine(ConnectedAnchorWorld, transform.position, Color.green);
    }
}
