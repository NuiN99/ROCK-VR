using NuiN.Movement;
using NuiN.NExtensions;
using SpleenTween;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwingingXR : MonoBehaviour
{
    Rigidbody _defaultConnectionPoint;
    Rigidbody _connectionPoint;
    bool _addedRigidbody;
    bool _attached;

    float _distFromStartHandLocalPosLastFrame;
    Vector3 _localHandPosLastFrame;

    GroundMovement _groundMovement;

    [SerializeField] SimpleTimer pullTimer;
    [SerializeField] float pullDistThreshold = 0.1f;

    [SerializeField] float zeroDragDurationAfterPull;

    [SerializeField] Transform root;
    
    [SerializeField] float maxAttachDistance = 25f;
    
    [SerializeField] InputActionReference activateAction;
    [SerializeField] LayerMask attachableLayers;

    [SerializeField] ConfigurableJoint joint;

    [SerializeField] LineRenderer indicatorLineRenderer;
    [SerializeField] LineRenderer ropeLineRenderer;

    [SerializeField] float pullForce = 5f;
    [SerializeField] Rigidbody rb;

    [SerializeField] bool useMouseAndKeyboard;
    
    void Awake()
    {
        _groundMovement = GetComponent<GroundMovement>();
        _defaultConnectionPoint = new GameObject("RopeConnectionPoint").AddComponent<Rigidbody>();
    }

    void Start()
    {
        indicatorLineRenderer.positionCount = 2;
        ropeLineRenderer.positionCount = 2;
        ropeLineRenderer.enabled = false;
        
        _defaultConnectionPoint.isKinematic = true;
        _defaultConnectionPoint.detectCollisions = false;
    }

    void Update()
    {
        if (activateAction.action.WasPressedThisFrame()) Activate();
        else if(activateAction.action.WasReleasedThisFrame()) Detach();

        if (!_attached)
        {
            joint.connectedAnchor = root.position;
        }
        else
        {
            joint.anchor = root.localPosition;
        }
    }

    void FixedUpdate()
    {
        if (_attached)
        {
            float distFromLastFrameHandPos = Vector3.Distance(root.localPosition, _localHandPosLastFrame);

            bool pulling = distFromLastFrameHandPos >= pullDistThreshold;

            if (pulling && pullTimer.Complete())
            {
                Vector3 dirFromLastFrameHandPos = transform.TransformDirection(VectorUtils.Direction(root.localPosition, _localHandPosLastFrame));
                Vector3 dirToConnection = VectorUtils.Direction(root.position, _connectionPoint.position);
                Vector3 combinedDir = (dirFromLastFrameHandPos + dirToConnection).normalized;

                if (_connectionPoint != _defaultConnectionPoint)
                {
                    _connectionPoint.AddForce((-dirFromLastFrameHandPos + -dirToConnection).normalized * pullForce / 5);

                    _groundMovement.disableGroundDrag = true;
                    Spleen.DoAfter(zeroDragDurationAfterPull, () => _groundMovement.disableGroundDrag = false);
                }
                else
                {
                    rb.AddForce(combinedDir * (pullForce * distFromLastFrameHandPos), ForceMode.VelocityChange);
                }
            }
        }
        
        _localHandPosLastFrame = root.localPosition;
    }

    void LateUpdate()
    {
        if (!_attached)
        {
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
            _connectionPoint = _defaultConnectionPoint;
            
            _connectionPoint.MovePosition(hit.point);
            _connectionPoint.transform.position = hit.point;
            
            joint.connectedBody = _connectionPoint;
            joint.connectedAnchor = _connectionPoint.transform.InverseTransformPoint(hit.point);
            
            _connectionPoint.transform.SetParent(hit.collider.transform);
        }
        else
        {
            _connectionPoint = attachedRB;
            
            joint.connectedBody = attachedRB;
            joint.connectedAnchor = attachedRB.transform.InverseTransformPoint(hit.point);
        }

        _localHandPosLastFrame = root.localPosition;
        
        SetJointLimit(Vector3.Distance(transform.position, hit.point));
        
        ropeLineRenderer.enabled = true;
        indicatorLineRenderer.enabled = false;
        
        _attached = true;
    }

    void SetJointLimit(float distance)
    {
        SoftJointLimit limit = joint.linearLimit;
        limit.limit = distance;
        joint.linearLimit = limit;
    }

    void Detach()
    {
        if (_connectionPoint == _defaultConnectionPoint)
        {
            _connectionPoint.transform.SetParent(null);
        }
        
        joint.connectedBody = null;

        ropeLineRenderer.enabled = false;
        indicatorLineRenderer.enabled = true;

        _attached = false;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(_localHandPosLastFrame), 0.1f);
        Gizmos.color = Color.white;

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