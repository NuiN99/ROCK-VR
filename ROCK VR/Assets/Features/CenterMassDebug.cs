using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterMassDebug : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float size = 0.02f;

    void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnDrawGizmos()
    {
        if (!rb) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(rb.centerOfMass), size);
        Gizmos.color = Color.white;
    }
}
