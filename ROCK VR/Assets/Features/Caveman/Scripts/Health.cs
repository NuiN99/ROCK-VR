using System;
using System.Collections;
using System.Collections.Generic;
using TNRD;
using UnityEngine;

public class Health : MonoBehaviour, IHealth
{
    [SerializeField] SerializableInterface<IDamageable> damageable;
    [SerializeField] float currenthealth;
    [SerializeField] float maxHealth = 100;
    
    public bool Dead { get; private set; }

    void Start()
    {
        currenthealth = maxHealth;

    }

    public void TakeDamage(float damage, Vector3 direction)
    {
        if (Dead) return;
        
        currenthealth -= damage;
        damageable.Value.Damaged(damage, direction);
        
        if (!(currenthealth <= 0)) return;
        
        damageable.Value.Died();
        Dead = true;
    }
}