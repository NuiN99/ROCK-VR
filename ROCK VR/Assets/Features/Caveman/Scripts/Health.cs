using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float _currenthealth;
    [SerializeField] float _maxHealth = 100;

    private void Start()
    {
        _currenthealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currenthealth -= damage;
        if (_currenthealth <= 0) 
        {
            Debug.Log(gameObject.name + " has died.");                        
        } 
        else Debug.Log(gameObject.name + " has " + _currenthealth + " remaining.");
    }

}
