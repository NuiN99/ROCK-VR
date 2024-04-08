using TNRD;
using UnityEngine;

public class HealthProxy : MonoBehaviour, IHealth
{
    [SerializeField] Health realHealth;
    [SerializeField] SerializableInterface<IDamageable> damageable;
    
    public void TakeDamage(float amount, Vector3 direction)
    {
        if (realHealth.Dead) return;
        damageable?.Value?.Damaged(amount, direction);
        realHealth.TakeDamage(amount, direction);
    }

    void OnValidate()
    {
        damageable.Value = GetComponent<IDamageable>();
    }
}