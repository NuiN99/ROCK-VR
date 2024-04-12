using TNRD;
using UnityEngine;

public class CavemanLimb : MonoBehaviour
{
    const float RAGDOLLING_DAMAGE_MULT = 2f;

    [SerializeField] ActiveRagdoll ragdoll;
    [SerializeField] SerializableInterface<IHealth> health;
    [SerializeField] Rigidbody rb;

    public ActiveRagdoll ParentRagdoll => ragdoll;
    
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) return;
        
        if (other.collider.TryGetComponent(out CavemanLimb otherLimb) && otherLimb.ParentRagdoll == ragdoll) return;

        if (other.collider.TryGetComponent(out Rigidbody otherRB))
        {
            health.Value.TakeDamage(other.relativeVelocity.magnitude * otherRB.mass, otherRB.velocity.normalized);
        }
        else if (ragdoll.Ragdolling)
        {
            health.Value.TakeDamage(rb.velocity.magnitude * RAGDOLLING_DAMAGE_MULT, rb.velocity.normalized);
        }
    }

    void OnValidate()
    {
        health.Value = GetComponent<IHealth>();
        rb = GetComponent<Rigidbody>();
    }
}
