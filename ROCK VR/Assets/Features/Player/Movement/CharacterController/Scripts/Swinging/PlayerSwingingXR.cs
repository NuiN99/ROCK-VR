using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwingingXR : MonoBehaviour
{
    Rigidbody _connectionPoint;
    bool _addedRigidbody;
    bool _attached;

    [SerializeField] Transform root;
    
    [SerializeField] float maxAttachDistance = 25f;
    
    [SerializeField] InputActionReference activateAction;
    [SerializeField] LayerMask attachableLayers;

    [SerializeField] ConfigurableJoint joint;

    [SerializeField] LineRenderer indicatorLineRenderer;
    [SerializeField] LineRenderer ropeLineRenderer;

    [SerializeField] bool useMouseAndKeyboard;
    
    void Awake()
    {
        indicatorLineRenderer.positionCount = 2;
        ropeLineRenderer.positionCount = 2;
        ropeLineRenderer.enabled = false;
        
        _connectionPoint = new GameObject("RopeConnectionPoint").AddComponent<Rigidbody>();
        _connectionPoint.isKinematic = true;
        _connectionPoint.detectCollisions = false;
    }

    void Update()
    {
        if (activateAction.action.WasPressedThisFrame()) Activate();
        else if(activateAction.action.WasReleasedThisFrame()) Detach();

        if (!_attached)
        {
            joint.connectedAnchor = root.position;
            
            indicatorLineRenderer.SetPosition(0, root.position);
            indicatorLineRenderer.SetPosition(1, root.position + root.forward * maxAttachDistance);
        }
        else
        {
            ropeLineRenderer.SetPosition(0, root.position);
            ropeLineRenderer.SetPosition(1, _connectionPoint.position);
        }
    }

    void Activate()
    {
        if (!Physics.Raycast(root.position, useMouseAndKeyboard ? MainCamera.Cam.transform.forward : root.forward, out RaycastHit hit, maxAttachDistance, attachableLayers)) return;
        if (!hit.collider.TryGetComponent(out Rigidbody attachedRB))
        {
            _connectionPoint.MovePosition(hit.point);
            _connectionPoint.transform.position = hit.point;
            
            joint.connectedBody = _connectionPoint;
            joint.connectedAnchor = _connectionPoint.transform.InverseTransformPoint(hit.point);
            
            _connectionPoint.transform.SetParent(hit.collider.transform);
        }
        else
        {
            joint.connectedBody = attachedRB;
            joint.connectedAnchor = attachedRB.transform.InverseTransformPoint(hit.point);
        }
        
        SoftJointLimit limit = joint.linearLimit;
        limit.limit = Vector3.Distance(transform.position, hit.point);
        joint.linearLimit = limit;
        
        ropeLineRenderer.enabled = true;
        indicatorLineRenderer.enabled = false;
        
        _attached = true;
    }

    void Detach()
    {
        _connectionPoint.transform.SetParent(null);
        
        joint.connectedBody = null;

        ropeLineRenderer.enabled = false;
        indicatorLineRenderer.enabled = true;

        _attached = false;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        if (_attached)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_connectionPoint.transform.position, 0.25f);
            Gizmos.color = Color.white;
            return;
        }
        
        if (!Physics.Raycast(root.position, useMouseAndKeyboard ? MainCamera.Cam.transform.forward : root.forward, out RaycastHit hit, maxAttachDistance, attachableLayers)) return;
        Gizmos.DrawSphere(hit.point, 0.25f);
    }
}