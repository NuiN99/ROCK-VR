using NuiN.ScriptableHarmony.Sound;
using System.Collections;
using System.Collections.Generic;
using NuiN.NExtensions;
using UnityEngine;

public class Caveman : MonoBehaviour, IDamageable
{
    [SerializeField] ActiveRagdoll ragdoll;
    [SerializeField] SoundSO injure;
    [SerializeField] SoundSO dead;
    [SerializeField] Transform head;

    [SerializeField] SimpleTimer damageSoundInterval;

    [SerializeField] GameObject eyelids;
    
    void IDamageable.Damaged(float amount, Vector3 direction)
    {
        if (damageSoundInterval.Complete())
        {
            injure.PlaySpatial(head.position);
        }
    }

    void IDamageable.Died()
    {
        eyelids.SetActive(true);
        dead.PlaySpatial(head.position);
        ragdoll.PermaRagdoll();
    }
}
