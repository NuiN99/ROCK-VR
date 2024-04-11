using UnityEngine;

public class PhysicalRig : MonoBehaviour
{
    [SerializeField] Transform head;
    [SerializeField] CapsuleCollider col;

    [SerializeField] float bodyHeightMin = 0.5f;
    [SerializeField] float bodyHeightMax = 2f;

    void FixedUpdate()
    {
        col.height = Mathf.Clamp(bodyHeightMin + head.localPosition.y, bodyHeightMin, bodyHeightMax);
        col.center = new Vector3(head.localPosition.x, col.height / 2, head.localPosition.z);
    }
}
