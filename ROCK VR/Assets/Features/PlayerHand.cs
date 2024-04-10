using System.Collections.Generic;
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
    
    Rigidbody _grabbedRB;
    RBSettings _grabbedRBSettings;

    void OnEnable()
    {
        grabAction.action.performed += Grab;
    }
    void OnDisable()
    {
        grabAction.action.performed -= Grab;
    }

    void FixedUpdate()
    {
        ApplySpringForce();
        ApplyRotationSpringForce();
    }

    void ApplySpringForce()
    {
        float force = Vector3.Distance(physicalHand.position, transform.position) * springStrength;
        Vector3 direction = VectorUtils.Direction(physicalHand.position, transform.position);
        
        physicalHand.AddForce((force * direction - physicalHand.velocity * springDamper));
    }

    void ApplyRotationSpringForce()
    {
        var springTorque = rotationSpringStrength * (Vector3.Cross(physicalHand.transform.up, transform.up) + Vector3.Cross(physicalHand.transform.forward, transform.forward));
        var dampTorque = rotationDamperStrength * -physicalHand.velocity;
        
        physicalHand.AddTorque((springTorque + dampTorque), ForceMode.Acceleration);
    }

    void Grab(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (_grabbedRB != null)
        {
            _grabbedRB.transform.parent = null;
            _grabbedRB.isKinematic = false;
            _grabbedRB.useGravity = true;
            _grabbedRB = null;
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, col.radius);
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

        _grabbedRB.useGravity = false;
        _grabbedRB.isKinematic = true;
        _grabbedRB.transform.parent = physicalHand.transform;
    }
}