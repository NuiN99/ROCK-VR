using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    const string VERTICAL = "Vertical";
    const string HORIZONTAL = "Horizontal";
    
    const string MOUSE_X = "Mouse X";
    const string MOUSE_Y = "Mouse Y";
    
    [SerializeField] float moveSpeed;
    [SerializeField] float mouseSens;
    
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;
    
    Vector2 _rotation = Vector2.zero;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float forward = Input.GetAxis(VERTICAL);
        transform.position += transform.forward * (moveSpeed * forward * Time.deltaTime);

        float horizontal = Input.GetAxis(HORIZONTAL);
        transform.position += transform.right * (moveSpeed * horizontal * Time.deltaTime);

        RotateCamera();
    }
    
    void RotateCamera()
    {
        _rotation.x += Input.GetAxis(MOUSE_X) * mouseSens;
        _rotation.y += Input.GetAxis(MOUSE_Y) * mouseSens;
        _rotation.y = Mathf.Clamp(_rotation.y, -yRotationLimit, yRotationLimit);
        var xQuat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(_rotation.y, Vector3.left);

        transform.localRotation = xQuat * yQuat;
    }
}
