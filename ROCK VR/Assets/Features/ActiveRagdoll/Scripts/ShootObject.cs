using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootObject : MonoBehaviour
{
    [SerializeField] Rigidbody prefab;
    [SerializeField] float shootForce;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Rigidbody obj = Instantiate(prefab, transform.position, Random.rotation);
            obj.velocity = transform.forward * shootForce;
        }
    }
}
