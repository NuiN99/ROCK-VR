using System.Collections;
using NuiN.NExtensions;
using SpleenTween;
using UnityEngine;

public class CullObject : MonoBehaviour
{
    const float CULL_DISTANCE = 100;
    const float CULL_INTERVAL = 1f;

    float DistFromPlayer => Vector3.Distance(transform.position, PlayerPosition.Value);
    
    void Start()
    {
        CullAndLoop();
    }

    void CullAndLoop()
    {
        bool cull = DistFromPlayer > CULL_DISTANCE;
        gameObject.SetActive(!cull);

        Spleen.DoAfter(CULL_INTERVAL, CullAndLoop);
    }
}
