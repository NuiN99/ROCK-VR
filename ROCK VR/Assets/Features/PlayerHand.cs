using System.Collections.Generic;
using System.Linq;
using Animancer;
using NuiN.NExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] InputActionReference grabAction;
    [SerializeField] SphereCollider col;

    [SerializeField] LayerMask grabMask;
    
    [Header("References")]
    [SerializeField] Rigidbody physicalHand;
    [SerializeField] PlayerHand otherHand;
    [SerializeField] Collider[] ignoreGrabbedObject;
    
    [Header("Animation")] 
    [SerializeField] AnimancerComponent animator;
    [SerializeField] AnimationClip openedAnim;
    [SerializeField] AnimationClip closedAnim;
    [SerializeField] float openFadeTime = 0.1f;
    [SerializeField] float closeFadeTime = 0.1f;

    ConfigurableJoint _grabJoint;
    Rigidbody _grabbedRB;
    RBSettings _grabbedRBSettings;
    bool _addedRB;
    
    void OnEnable()
    {
        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;
    }
    void OnDisable()
    {
        grabAction.action.performed -= Grab;
        grabAction.action.canceled -= Release;
    }
    
    void Grab(InputAction.CallbackContext context)
    {
        animator.Play(closedAnim, closeFadeTime).Force();
        
        Collider[] hits = Physics.OverlapSphere(transform.position, col.radius, grabMask);
        if (hits.Length <= 0) return;
        
        Collider closest = GeneralUtils.GetClosest(transform.position, hits.ToList());

        _addedRB = false;
        
        if (!closest.TryGetComponent(out _grabbedRB))
        {
            if (closest.GetComponentInParent<Rigidbody>())
            {
                _grabbedRB = closest.GetComponentInParent<Rigidbody>();
            }
            else
            {
                _grabbedRB = closest.gameObject.AddComponent<Rigidbody>();
                _addedRB = true;
                _grabbedRB.isKinematic = true;
                _grabbedRB.useGravity = false;
            }
        }
        else
        {
            _grabbedRB.interpolation = RigidbodyInterpolation.Interpolate;
        }

        _grabJoint = physicalHand.gameObject.AddComponent<ConfigurableJoint>();
        _grabJoint.autoConfigureConnectedAnchor = false;
        _grabJoint.connectedBody = _grabbedRB;
        _grabJoint.connectedAnchor = _grabbedRB.transform.InverseTransformPoint(physicalHand.transform.position);
        
        _grabJoint.xMotion = ConfigurableJointMotion.Locked;
        _grabJoint.yMotion = ConfigurableJointMotion.Locked;
        _grabJoint.zMotion = ConfigurableJointMotion.Locked;
        _grabJoint.angularXMotion = ConfigurableJointMotion.Locked;
        _grabJoint.angularYMotion = ConfigurableJointMotion.Locked;
        _grabJoint.angularZMotion = ConfigurableJointMotion.Locked;
        _grabJoint.projectionMode = JointProjectionMode.PositionAndRotation;

        _grabJoint.enableCollision = false;

        if (!_addedRB)
        {
            IgnoreCollisions(true);
        }
    }

    void Release(InputAction.CallbackContext context)
    {
        animator.Play(openedAnim, openFadeTime).Force();
        
        if (_grabbedRB == null) return;
        
        IgnoreCollisions(false);

        if (_addedRB && otherHand._grabbedRB != _grabbedRB)
        {
            _addedRB = false;
            Destroy(_grabbedRB);
        }
        
        Destroy(_grabJoint);
        _grabbedRB = null;
    }

    void IgnoreCollisions(bool ignore)
    {
        if (_grabbedRB == null) return;
        if (!_grabbedRB.TryGetComponent(out Collider grabbedCol)) return;
        
        foreach (var otherCol in ignoreGrabbedObject)
        {
            Physics.IgnoreCollision(grabbedCol, otherCol, ignore);
        }
    }
}