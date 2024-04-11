using UnityEngine;

public struct RBSettings
{
    public float mass;
    public float drag;
    public float angularDrag;
    public bool automaticCenterOfMass;
    public bool automaticInertiaTensor;
    public bool useGravity;
    public bool isKinematic;

    public RigidbodyInterpolation interpolation;
    public CollisionDetectionMode collisionDetectionMode;

    public RigidbodyConstraints constraints;

    public LayerMask includeLayers;
    public LayerMask excludeLayers;

    public void InitRB(Rigidbody rb)
    {
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.automaticCenterOfMass = automaticCenterOfMass;
        rb.automaticInertiaTensor = automaticInertiaTensor;
        rb.useGravity = useGravity;
        rb.isKinematic = isKinematic;
        rb.interpolation = interpolation;
        rb.collisionDetectionMode = collisionDetectionMode;
        rb.constraints = constraints;
        rb.includeLayers = includeLayers;
        rb.excludeLayers = excludeLayers;
    }
    
    public void SetValues(Rigidbody rb)
    {
        mass = rb.mass;
        drag = rb.drag;
        angularDrag = rb.angularDrag;
        automaticCenterOfMass = rb.automaticCenterOfMass;
        automaticInertiaTensor = rb.automaticInertiaTensor;
        useGravity = rb.useGravity;
        isKinematic = rb.isKinematic;
        interpolation = rb.interpolation;
        collisionDetectionMode = rb.collisionDetectionMode;
        constraints = rb.constraints;
        includeLayers = rb.includeLayers;
        excludeLayers = rb.excludeLayers;
    }
}