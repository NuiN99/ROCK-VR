using UnityEngine;

public interface IDamageable
{
    public void Damaged(float amount, Vector3 direction);
    public void Died();
}