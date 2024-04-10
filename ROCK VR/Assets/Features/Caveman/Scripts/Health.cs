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
            
        if (currenthealth <= 0)
        {
            damageable.Value.Died();
            Dead = true;
            Debug.Log($"<color=green>{gameObject.name}</color> | <color=black>Died</color>", gameObject);
        } 
        else Debug.Log($"<color=green>{gameObject.name}</color> | <color=red>{currenthealth}</color> Health", gameObject);
    }
}