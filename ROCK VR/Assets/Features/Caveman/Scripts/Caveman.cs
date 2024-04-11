using NuiN.ScriptableHarmony.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caveman : MonoBehaviour, IDamageable
{
    [SerializeField] ActiveRagdoll ragdoll;
    [SerializeField] SoundSO injure;
    [SerializeField] SoundSO dead;
    [SerializeField] Transform head;
    void IDamageable.Damaged(float amount, Vector3 direction)
    {
        injure.PlaySpatial(head.position);
        
    }

    void IDamageable.Died()
    {
        dead.PlaySpatial(head.position);
        ragdoll.PermaRagdoll();
    }
}
