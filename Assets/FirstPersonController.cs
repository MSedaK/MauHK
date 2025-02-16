using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonControllerScript : MonoBehaviour
{
    public int FPS = 120;
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    private Vector3 moveDirection = Vector3.zero;

    float yRotation;
    float xRotation;
    public float lookSensitivity = 2;
    float currentXRotation;
    float currentYRotation;
    float xRotationV;
    float yRotationV;
    public float lookSmoothness = 0.1f;

    void Start()
    {
        Application.targetFrameRate = FPS;
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();

        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
        xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;

        xRotation = Mathf.Clamp(xRotation, -80, 100);

        currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref xRotationV, lookSmoothness);
        currentYRotation = Mathf.SmoothDamp(currentYRotation, yRotation, ref yRotationV, lookSmoothness);

        transform.rotation = Quaternion.Euler(xRotation, currentYRotation, 0);
        Camera.main.transform.localRotation = Quaternion.Euler(currentXRotation, 0, 0);
    }
}