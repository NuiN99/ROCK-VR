using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float weaponDamage = 50;
    Vector3 lastPosition;
    Vector3 currentPosition;

    private void Update()
    {
        lastPosition = currentPosition;
        currentPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Health>(out Health health))
        {
            float impactSpeed = getImpactSpeed(lastPosition, currentPosition);
            Debug.Log("Impact Speed: " + impactSpeed);
            health.takeDamage(weaponDamage * impactSpeed);
        }
    }

    private float getImpactSpeed(Vector3 lastPos, Vector3 currentPos)
    {
        float speed =  (currentPos - lastPos).magnitude;
        return speed;
    }
}
