using System;
using System.Collections;
using System.Collections.Generic;
using NuiN.NExtensions;
using UnityEngine;
using SimpleTimer = NuiN.NExtensions.SimpleTimer;

public class CavemanBrain : MonoBehaviour
{
    public enum State
    {
        Idle,
        Chase,
        Attack,
        Search,
        Ragdoll,
        Dead
    }

    [SerializeField] CavemanAnimation anim;

    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    
    [SerializeField] Transform body;

    [SerializeField] SphereCollider attackBounds;
    [SerializeField] SphereCollider visionBounds;

    [SerializeField] SimpleTimer detectionInterval;

    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask noCavemanMask;

    State _currentState;
    Vector3 _lastSeenPos;

    void Update()
    {
        SnapToGround();
        body.position = Vector3.MoveTowards(body.position, _lastSeenPos, moveSpeed * Time.deltaTime);
        Vector3 direction = VectorUtils.Direction(body.position, _lastSeenPos);
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            body.rotation = Quaternion.RotateTowards(body.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }
        
        if (!detectionInterval.Complete()) return;

        Collider[] possibleColliders = Physics.OverlapSphere(visionBounds.transform.position + visionBounds.center, visionBounds.radius, playerMask);
        bool detectedPlayer = possibleColliders.Length > 0;
        
        if (detectedPlayer && _currentState is State.Search or State.Attack or State.Chase)
        {
            _lastSeenPos = possibleColliders[0].transform.position;
        }

        if (_currentState is State.Search or State.Idle)
        {
            if (Physics.OverlapSphere(visionBounds.transform.position + visionBounds.center, visionBounds.radius, playerMask).Length > 0)
            {
                Debug.Log("Detected Player", gameObject);
                SetState(State.Chase);
            }
        }
    }

    void SetState(State state)
    {
        anim.PlayAnimation(state);
        _currentState = state;
    }

    Color GetStateColor()
    {
        return _currentState switch
        {
            State.Idle => Color.green,
            State.Search => Color.yellow,
            State.Attack => Color.blue,
            State.Chase => Color.red,
            State.Dead => Color.black,
            State.Ragdoll => Color.black,
            _ => Color.white
        };
    }

    void SnapToGround()
    {
        if (_currentState != State.Ragdoll && _currentState != State.Dead)
        {
            if (Physics.Raycast(body.position + (Vector3.up / 2), Vector3.down, out RaycastHit hit, 10, noCavemanMask))
            {
                body.transform.position = hit.point;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = GetStateColor();
        if (visionBounds != null)
        {
            Gizmos.DrawWireSphere(visionBounds.transform.position + visionBounds.center, visionBounds.radius);
        }
        Gizmos.color = Color.white;
    }
}