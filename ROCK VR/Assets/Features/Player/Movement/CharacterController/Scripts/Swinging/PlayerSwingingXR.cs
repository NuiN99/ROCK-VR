using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwingingXR : MonoBehaviour
{
    Transform _connectionPoint;
    Rigidbody _anchor;
    bool _addedRigidbody;
    bool _attached;

    [SerializeField] Transform root;
    
    [SerializeField] float maxAttachDistance = 25f;
    
    [SerializeField] InputActionReference activateAction;
    [SerializeField] LayerMask attachableLayers;

    [SerializeField] ConfigurableJoint joint;

    [SerializeField] LineRenderer indicatorLineRenderer;
    [SerializeField] LineRenderer ropeLineRenderer;
    
    Vector3 ConnectedAnchorWorld => _anchor.transform.TransformPoint(joint.connectedAnchor);

    void Awake()
    {
        indicatorLineRenderer.positionCount = 2;
        ropeLineRenderer.positionCount = 2;
        ropeLineRenderer.enabled = false;
        _connectionPoint = new GameObject("RopeConnectionPoint").transform;
    }

    void Update()
    {
        if (activateAction.action.WasPressedThisFrame()) Activate();
        else if(activateAction.action.WasReleasedThisFrame()) Detach();

        if (!_attached)
        {
            joint.connectedAnchor = transform.position;
            
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
        if (!Physics.Raycast(root.position, root.forward, out RaycastHit hit, maxAttachDistance, attachableLayers)) return;
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

        _connectionPoint.position = hit.point;
        _connectionPoint.SetParent(hit.collider.transform);
        
        ropeLineRenderer.enabled = true;
        indicatorLineRenderer.enabled = false;
        
        _attached = true;
    }

    void Detach()
    {
        joint.connectedBody = null;
        if(_anchor != null && _addedRigidbody) Destroy(_anchor);

        ropeLineRenderer.enabled = false;
        indicatorLineRenderer.enabled = true;

        _attached = false;
    }

    void OnDrawGizmos()
    {
        if (!_attached)
        {
            if (!Physics.Raycast(root.position, root.forward, out RaycastHit hit, maxAttachDistance, attachableLayers)) return;
            Gizmos.DrawSphere(hit.point, 0.25f);
            return;
        }
        Debug.DrawLine(ConnectedAnchorWorld, transform.position, Color.green);
    }
}