using System.Collections;
using System.Diagnostics;
using System.Linq;
using Animancer;
using NuiN.NExtensions;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ActiveRagdoll : MonoBehaviour
{
    bool _fullRagdoll;
    
    [Header("Force Settings")]
    [SerializeField] float globalMoveForce = 0.7f;
    [SerializeField] float globalRotateForce = 25f;
    [SerializeField, Range(0f, 1f)] float damping = 0.75f;

    [Header("Ragdoll Settings")]
    [SerializeField] float maxOffBalanceDist = 0.3f;
    [SerializeField] float getUpAfterRagdolledDelay = 2.5f;
    [SerializeField] Transform animatedRigRoot;
    [SerializeField] float getUpGroundDistReq = 1f;
    [SerializeField] float getUpVelocityThreshold = 1f;

    [Header("Transform Positions")] 
    [SerializeField] Rigidbody physicalHips;
    [SerializeField] Transform animatedHips;
    [SerializeField] Transform frontPos;
    [SerializeField] Transform backPos;

    [Header("Ground Check")]
    [SerializeField] float groundCheckDist = 0.25f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform leftFootPhysical;
    [SerializeField] Transform rightFootPhysical;
    
    [Header("Animation")]
    [SerializeField] AnimancerComponent animator;
    [SerializeField] AnimationClip walkAnim;
    [SerializeField] AnimationClip getUpFromBackDownAnim;
    [SerializeField] AnimationClip getUpFromFaceDownAnim;
    [SerializeField] AnimationClip idleAnim;
    
    [Header("Limb Force")]
    [SerializeField] FollowLimb[] limbs;
    
    void Start()
    {
        animator.Play(idleAnim, 1f).Force();
    }

    void FixedUpdate()
    {
        if (_fullRagdoll) return;
        
        foreach (var limb in limbs)
        {
            limb.MoveToTarget(globalMoveForce, globalRotateForce, damping);
        }
        
        if (OffBalance() || !FeetTouchingGround())
        {
            Ragdoll();
        }
    }

    bool FeetTouchingGround()
    {
        bool leftHit = Physics.Raycast(leftFootPhysical.position, Vector3.down, groundCheckDist, groundMask);
        bool rightHit = Physics.Raycast(rightFootPhysical.position, Vector3.down, groundCheckDist, groundMask);
        
        DrawGroundCheckGizmos(leftHit, rightHit);
        
        return leftHit || rightHit;
    }

    bool OffBalance()
    {
        float offBalanceDist = Vector3.Distance(physicalHips.position, animatedHips.position);
        bool offBalance = offBalanceDist >= maxOffBalanceDist;
        return offBalance;
    }

    void Ragdoll()
    {
        animator.Stop();
        _fullRagdoll = true;
        StartCoroutine(GetUpAfterDelay(getUpAfterRagdolledDelay));
    }

    IEnumerator GetUpAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (physicalHips.velocity.magnitude >= getUpVelocityThreshold || 
            !Physics.Raycast(physicalHips.position, Vector3.down, out RaycastHit hit, getUpGroundDistReq, groundMask))
        {
            StartCoroutine(GetUpAfterDelay(delay));
            yield break;
        }

        animatedRigRoot.position = hit.point;
        animatedRigRoot.rotation = Quaternion.Euler(0, physicalHips.rotation.eulerAngles.y, 0);
        
        bool lyingFaceDown = frontPos.position.y < backPos.position.y;
        AnimationClip getUpAnim = lyingFaceDown ? getUpFromFaceDownAnim : getUpFromBackDownAnim;
        animator.Play(getUpAnim, 1f).Force()
            .OnComplete(() => animator.Play(walkAnim, 1f));
        
        _fullRagdoll = false;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (_fullRagdoll)
        {
            Transform getUpFrom = frontPos.position.y <= backPos.position.y ? frontPos : backPos;
            
            Gizmos.color = Color.green;
            
            Gizmos.DrawSphere(getUpFrom.position, 0.1f);
            Gizmos.DrawRay(physicalHips.position, Vector3.down * getUpGroundDistReq);
            
            Gizmos.color = Color.white;
            return;
        }
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(physicalHips.position, 0.1f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(animatedHips.position, 0.1f);
        
        Gizmos.DrawLine(physicalHips.position, animatedHips.position);

        Gizmos.color = Color.white;
    }

    [Conditional("UNITY_EDITOR")]
    void DrawGroundCheckGizmos(bool leftFootStable, bool rightFootStable)
    {
        Debug.DrawRay(leftFootPhysical.position, Vector3.down * groundCheckDist, leftFootStable ? Color.green : Color.red);
        Debug.DrawRay(rightFootPhysical.position, Vector3.down * groundCheckDist, rightFootStable ? Color.green : Color.red);
    }
}