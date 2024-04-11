using UnityEngine;

public class PhysicalRig : MonoBehaviour
{
    [SerializeField] Transform head;
    [SerializeField] CapsuleCollider col;

    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;

    [SerializeField] ConfigurableJoint headJoint;
    [SerializeField] ConfigurableJoint leftHandJoint;
    [SerializeField] ConfigurableJoint rightHandJoint;
    
    [SerializeField] float bodyHeightMin = 0.5f;
    [SerializeField] float bodyHeightMax = 2f;

    void FixedUpdate()
    {
        col.height = Mathf.Clamp(head.localPosition.y, bodyHeightMin, bodyHeightMax);
        col.center = new Vector3(head.localPosition.x, col.height / 2, head.localPosition.z);

        leftHandJoint.targetPosition = leftHand.localPosition;
        leftHandJoint.targetRotation = leftHand.localRotation;

        rightHandJoint.targetPosition = rightHand.localPosition;
        rightHandJoint.targetRotation = rightHand.localRotation;
        
        headJoint.targetPosition = head.localPosition;
    }
}
