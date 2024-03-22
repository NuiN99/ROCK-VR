using NuiN.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementXRInput : MonoBehaviour, IMovementInput
{
    Vector2 _rotation;

    [SerializeField] InputActionReference rotationAction;
    [SerializeField] InputActionReference jumpAction;

    //[SerializeField] float jumpYThreshold = 0.25f;
    [SerializeField] float lookSensitivity = 20f;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    bool IMovementInput.ShouldJump()
    {
        return jumpAction.action.WasPressedThisFrame();
        //return jumpAction.action.ReadValue<Vector2>().y >= jumpYThreshold;
    }

    Vector3 IMovementInput.GetDirection()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 forward = MainCamera.Cam.transform.forward;
        Vector3 right = MainCamera.Cam.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = forward * z + right * x;

        return desiredMoveDirection.normalized;
    }
    
    Quaternion IMovementInput.GetRotation()
    {
        _rotation.x += rotationAction.action.ReadValue<Vector2>().x * lookSensitivity;
    
        var xQuat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(_rotation.y, Vector3.left);

        Quaternion newRotation = xQuat * yQuat;

        return Quaternion.Euler(0, newRotation.eulerAngles.y, 0);
    }

    bool IMovementInput.IsRunning()
    {
        return false;
    }
}