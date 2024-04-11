using System.Collections;
using System.Collections.Generic;
using NuiN.NExtensions;
using NuiN.ScriptableHarmony.Sound;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float weaponDamage = 50;
    [SerializeField] SoundSO hitSounds;
    Vector3 _lastPosition;
    Vector3 _currentPosition;

    private void FixedUpdate()
    {
        _lastPosition = _currentPosition;
        _currentPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IHealth health))
        {
            float impactSpeed = GetImpactSpeed(_lastPosition, _currentPosition);
            Debug.Log("Impact Speed: " + impactSpeed);
            health.TakeDamage(weaponDamage * impactSpeed, VectorUtils.Direction(_lastPosition, _currentPosition));
            hitSounds.Play();
        }
    }

    private float GetImpactSpeed(Vector3 lastPos, Vector3 currentPos)
    {
        float speed =  (currentPos - lastPos).magnitude;
        return speed;
    }
}
