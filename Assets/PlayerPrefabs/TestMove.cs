using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float lookSpeed = 2f;
    public Transform cameraTransform;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isJumping = false;
    private float yaw;
    private float pitch;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        yaw = transform.eulerAngles.y;
        pitch = cameraTransform.eulerAngles.x;
    }

    public void OnMove(InputAction.CallbackContext mv)
    {
        if (mv.started || mv.performed)
            moveInput = mv.ReadValue<Vector2>();
        else if (mv.canceled)
            moveInput = Vector2.zero;
    }

    public void OnLook(InputAction.CallbackContext lk)
    {
        lookInput = lk.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext jp)
    {
        if (jp.started && IsGrounded())
            isJumping = true;
    }

    private void FixedUpdate()
    {
        Move();
        if (isJumping)
        {
            Jump();
            isJumping = false;
        }
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        RotateView();
        cameraTransform.position = transform.position + Vector3.up * 1.5f;

    }

    private void Move()
    {
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
    }

    private void RotateView()
    {
        yaw += lookInput.x * lookSpeed;
        pitch -= lookInput.y * lookSpeed;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
