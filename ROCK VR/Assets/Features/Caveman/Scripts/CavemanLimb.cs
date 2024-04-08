using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavemanLimb : MonoBehaviour, IDamageable
{
    Rigidbody _rb;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Damaged(float amount, Vector3 direction)
    {
        _rb.AddForce(direction * amount, ForceMode.Impulse);
    }

    public void Died()
    {
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent(out Rigidbody rb))
        {
            Damaged(rb.mass, rb.velocity.normalized);
        }
    }
}
