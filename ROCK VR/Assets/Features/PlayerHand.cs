using System.Collections.Generic;
using Animancer;
using NuiN.NExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] InputActionReference grabAction;
    [SerializeField] SphereCollider col;

    [SerializeField] LayerMask grabMask;
    
    [Header("Physical Properties")]
    [SerializeField] Rigidbody physicalHand;

    [Header("Animation")] 
    [SerializeField] AnimancerComponent animator;
    [SerializeField] AnimationClip openedAnim;
    [SerializeField] AnimationClip closedAnim;
    [SerializeField] float openFadeTime = 0.1f;
    [SerializeField] float closeFadeTime = 0.1f;

    int _grabbedObjLayer;
    ConfigurableJoint _grabJoint;
    Rigidbody _grabbedRB;
    RBSettings _grabbedRBSettings;

    [SerializeField] Collider[] ignoreGrabbedObject;
    
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
        
        List<Rigidbody> hitBodies = new();
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Rigidbody rb))
            {
                if (rb == physicalHand) continue;
                hitBodies.Add(rb);
            }
        }
        
        if (hitBodies.Count <= 0) return;
        
        _grabbedRB = GeneralUtils.GetClosest(transform.position, hitBodies);

        _grabbedRB.interpolation = RigidbodyInterpolation.Interpolate;

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

        _grabbedObjLayer = _grabbedRB.gameObject.layer;
        
        IgnoreCollisions(true);

        Debug.Log("Player Grabbed: " + _grabbedRB.name);
    }

    void Release(InputAction.CallbackContext context)
    {
        animator.Play(openedAnim, openFadeTime).Force();
        
        if (_grabbedRB == null) return;

        IgnoreCollisions(false);
        _grabbedObjLayer = 0;
        
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