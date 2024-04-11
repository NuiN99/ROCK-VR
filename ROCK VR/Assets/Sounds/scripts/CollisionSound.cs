using NuiN.NExtensions;
using NuiN.ScriptableHarmony.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    const float MIN_FORCE = 5f;
    [SerializeField] SoundSO hitSound;
    [SerializeField] SimpleTimer timer;

    private void OnCollisionEnter(Collision collision)
    {
        if (!timer.Complete()) return;
        if (hitSound == null) return;


        float mag = collision.relativeVelocity.magnitude;
        if (mag < MIN_FORCE) return;
        hitSound.PlaySpatial(collision.contacts[0].point, null, mag);
    }
}

