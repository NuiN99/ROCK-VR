using TNRD;
using UnityEngine;

public class HealthProxy : MonoBehaviour, IHealth
{
    [SerializeField] float damageMult = 1f;
    [SerializeField] Health realHealth;
    [SerializeField] SerializableInterface<IDamageable> damageable;
    
    public void TakeDamage(float amount, Vector3 direction)
    {
        if (realHealth.Dead) return;
        damageable?.Value?.Damaged(amount * damageMult, direction);
        realHealth.TakeDamage(amount * damageMult, direction);
    }

    void OnValidate()
    {
        damageable.Value = GetComponent<IDamageable>();
    }
}