using System;
using System.Collections;
using System.Collections.Generic;
using NuiN.NExtensions;
using SpleenTween;
using UnityEngine;
using SimpleTimer = NuiN.NExtensions.SimpleTimer;

public class CavemanBrain : MonoBehaviour, IActiveRagdoll
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

    [SerializeField] GameObject club;

    [SerializeField] Bounds detectionBounds;

    [SerializeField] CavemanAnimation anim;

    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    
    [SerializeField] Transform body;
    [SerializeField] Transform physicalBody;
    [SerializeField] float maxDistFromPhysical = 0.25f;
    [SerializeField] float attackDistance = 1f;

    [SerializeField] SimpleTimer detectionInterval;

    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask noCavemanMask;

    [SerializeField, ReadOnlyPlayMode] State currentState;
    Vector3 _lastSeenPos;

    [SerializeField] float attackDuration = 2f;

    void Update()
    {
        if (currentState is State.Ragdoll or State.Dead) return;
        
        SnapToGround();
        Move();
        
        if (!detectionInterval.Complete()) return;

        bool detectedPlayer = TryDetectPlayer(out Collider[] hitColliders);
        bool inRadius = Vector3.Distance(body.position, PlayerPosition.Value) <= attackDistance * 5;

        if (currentState != State.Attack && (inRadius || (detectedPlayer && currentState is State.Chase)))
        {
            if (Vector3.Distance(body.position, PlayerPosition.Value) <= attackDistance)
            {
                SetState(State.Attack);
                Spleen.DoAfter(attackDuration, () => SetState(State.Idle));
            }
        }
        
        if (detectedPlayer && currentState is State.Search or State.Attack or State.Chase)
        {
            _lastSeenPos = hitColliders[0].transform.position;
        }

        if (currentState is State.Search or State.Idle)
        {
            if (detectedPlayer || inRadius)
            {
                SetState(State.Chase);
            }
        }
    }

    public void SetState(State state)
    {
        anim.PlayAnimation(state);
        currentState = state;
    }
    
    void SnapToGround()
    {
        if (currentState != State.Ragdoll && currentState != State.Dead)
        {
            if (Physics.Raycast(body.position + (Vector3.up / 2), Vector3.down, out RaycastHit hit, 10, noCavemanMask))
            {
                body.transform.position = hit.point - new Vector3(0, 0.05f, 0f);
            }
        }
    }

    void Move()
    {
        if (_lastSeenPos == Vector3.zero || currentState != State.Chase && currentState != State.Search) return;
        if (Vector3.Distance(body.position.With(y:0), physicalBody.position.With(y:0)) >= maxDistFromPhysical) return;
        
        body.position = Vector3.MoveTowards(body.position, _lastSeenPos, moveSpeed * Time.deltaTime);
        
        Vector3 direction = VectorUtils.Direction(body.position, _lastSeenPos.With(y: body.position.y));
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        body.rotation = Quaternion.RotateTowards(body.rotation, lookRotation, rotateSpeed * Time.deltaTime);
    }

    bool TryDetectPlayer(out Collider[] colliders)
    {
        Vector3 halfExtents = new Vector3(detectionBounds.size.x / 2f, detectionBounds.size.y / 2f, detectionBounds.size.z / 2f);
        Quaternion boxRotation = Quaternion.LookRotation(body.forward);
        colliders = Physics.OverlapBox(body.TransformPoint(detectionBounds.center), halfExtents, boxRotation, playerMask);
        bool detectedPlayer = colliders.Length > 0;
        return detectedPlayer;
    }

    Color GetStateColor()
    {
        return currentState switch
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

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(body.position, 0.15f);
        
        Vector3 halfExtents = new Vector3(detectionBounds.size.x / 2f, detectionBounds.size.y / 2f, detectionBounds.size.z / 2f);
        Quaternion boxRotation = Quaternion.LookRotation(body.forward);
        Gizmos.color = GetStateColor();
        Gizmos.matrix = Matrix4x4.TRS(body.TransformPoint(detectionBounds.center), boxRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
        
        Gizmos.color = Color.white;
    }

    public void Ragdolled()
    {
        SetState(State.Ragdoll);
    }

    public void UnRagdolled()
    {
        SetState(State.Idle);
    }

    public void Died()
    {
        club.transform.parent = null;
        club.AddComponent<Rigidbody>().mass = 3;
        
        SetState(State.Dead);
    }
}