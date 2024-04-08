using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caveman : MonoBehaviour, IDamageable
{
    [SerializeField] ActiveRagdoll ragdoll;
    
    void IDamageable.Damaged(float amount, Vector3 direction)
    {
        //ragdoll.AddForceInDirection(direction, amount);
    }

    void IDamageable.Died()
    {
        ragdoll.PermaRagdoll();
    }
}
