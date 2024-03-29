using System;
using UnityEngine;

[Serializable]
public class FollowLimb
{
    [field: SerializeField] public Rigidbody RB { get; private set; }
    [field: SerializeField] public Transform Target { get; private set; }
    [field: SerializeField, Range(0f, 1f)] public float MoveForce { get; private set; } = 1f;
    [field: SerializeField, Range(0f, 1f)] public float RotateForce { get; private set; } = 1f;

    [SerializeField] float rotateAdjustFactor = 0.5f;

    public void MoveToTarget(float moveMult, float rotateMult, float damping)
    {
        RotateToTarget(rotateMult);
        
        Vector3 direction = (Target.position - RB.position).normalized;
        RB.velocity += direction * (MoveForce * moveMult);
        RB.velocity *= damping;
    }

    void RotateToTarget(float rotateMult)
    {
        Quaternion deltaQuat = Quaternion.FromToRotation(RB.transform.forward, Target.transform.forward);
        deltaQuat.ToAngleAxis(out float angle, out Vector3 axis);
        RB.AddTorque(-RB.angularVelocity * (RotateForce * rotateMult), ForceMode.Acceleration);
        RB.AddTorque(axis.normalized * (angle * rotateAdjustFactor * rotateMult), ForceMode.Acceleration);
    }
}