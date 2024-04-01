using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float weaponDamage = 50;
    Vector3 lastPosition;
    Vector3 currentPosition;

    private void FixedUpdate()
    {
        lastPosition = currentPosition;
        currentPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Health>(out Health health))
        {
            float impactSpeed = GetImpactSpeed(lastPosition, currentPosition);
            Debug.Log("Impact Speed: " + impactSpeed);
            health.TakeDamage(weaponDamage * impactSpeed);
        }
    }

    private float GetImpactSpeed(Vector3 lastPos, Vector3 currentPos)
    {
        float speed =  (currentPos - lastPos).magnitude;
        return speed;
    }
}
