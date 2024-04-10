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
    [SerializeField] float springStrength = 25f;
    [SerializeField] float springDamper = 5f;
    [SerializeField] float rotationSpringStrength = 25f;
    [SerializeField] float rotationDamperStrength = 5f;

    [Header("Animation")] 
    [SerializeField] AnimancerComponent animator;
    [SerializeField] AnimationClip openedAnim;
    [SerializeField] AnimationClip closedAnim;
    [SerializeField] float openFadeTime = 0.1f;
    [SerializeField] float closeFadeTime = 0.1f;

    ConfigurableJoint _grabJoint;
    Rigidbody _grabbedRB;
    RBSettings _grabbedRBSettings;


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

    void FixedUpdate()
    {
        ApplyRotationSpringForce();
        ApplySpringForce();
    }

    void ApplyRotationSpringForce()
    {
        Vector3 rotation = 
            Vector3.Cross(physicalHand.transform.up, transform.up) + 
            Vector3.Cross(physicalHand.transform.forward, transform.forward) + 
            Vector3.Cross(physicalHand.transform.right, transform.right);
        var springTorque = rotationSpringStrength * rotation;
        var dampTorque = rotationDamperStrength * -physicalHand.angularVelocity;
        
        physicalHand.AddTorque((springTorque + dampTorque), ForceMode.Acceleration);
        
        // QUATERNION IMPLEMENTATION
        /*
        Quaternion targetRotation = transform.rotation;
        Quaternion currentRotation = physicalHand.transform.rotation;
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);

        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180)
            angle -= 360;

        angle *= Mathf.Deg2Rad;

        Vector3 springTorque = angle * axis.normalized * rotationSpringStrength;

        Vector3 dampTorque = -rotationDamperStrength * physicalHand.angularVelocity;

        physicalHand.AddTorque(springTorque + dampTorque, ForceMode.Acceleration);*/
    }
    
    void ApplySpringForce()
    {
        float force = Vector3.Distance(physicalHand.position, transform.position) * springStrength;
        Vector3 direction = VectorUtils.Direction(physicalHand.position, transform.position);
        
        physicalHand.AddForce((force * direction - physicalHand.velocity * springDamper));
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
        
        //Physics.IgnoreCollision(_grabbedRB.GetComponent<Collider>(), physicalHand.GetComponent<Collider>(), true);
        
        Debug.Log("Player Grabbed: " + _grabbedRB.name);
    }

    void Release(InputAction.CallbackContext context)
    {
        animator.Play(openedAnim, openFadeTime).Force();
        
        if (_grabbedRB == null) return;

        //Physics.IgnoreCollision(_grabbedRB.GetComponent<Collider>(), physicalHand.GetComponent<Collider>(), false);
        
        Destroy(_grabJoint);
        _grabbedRB = null;
    }
}