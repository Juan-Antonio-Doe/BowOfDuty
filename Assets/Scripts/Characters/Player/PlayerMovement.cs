using Nrjwolf.Tools.AttachAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the player's movement.
/// Uses a Rigidbody component to move the player.
/// </summary>

public class PlayerMovement : MonoBehaviour {

    [field: Header("- Autoattach propierties -")]
    [field: SerializeField, GetComponent, ReadOnlyField] private Rigidbody rb { get; set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private CapsuleCollider playerCollider { get; set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private PlayerWallRun playerWallRun { get; set; }

    [field: Header("Player stats")]
    [field: SerializeField, ReadOnlyField] private float playerHeight { get; set; } = 2f;
    private float halfPlayerHeight { get; set; }
    [field: SerializeField] private Transform groundCheck { get; set; }
    // groundMask by default is the 0 and 6 layers (Default and Ground)
    [field: SerializeField] private LayerMask groundMask { get; set; } = 1 << 0 | 1 << 6;

    [field: Header("Movement settings")]
    [field: SerializeField] private Transform orientation { get; set; }
    [field: SerializeField] private float moveSpeed { get; set; } = 6f;
    [field: SerializeField] private float movementMultiplier { get; set; } = 10f;
    [field: SerializeField] private float groundDrag { get; set; } = 6f;

    [field: Header("Spring settings")]
    [field: SerializeField] private float walkSpeed { get; set; } = 4f;
    [field: SerializeField] private float sprintSpeed { get; set; } = 6f;
    [field: SerializeField] private float acceleration { get; set; } = 10f;
    [field: SerializeField, ReadOnlyField] private bool isSprinting { get; set; }

    [field: Header("Jump settings")]
    [field: SerializeField] private float airMovementMultiplier { get; set; } = 2.35f;
    [field: SerializeField] private float jumpForce { get; set; } = 5f;
    [field: SerializeField] private float airDrag { get; set; } = 2f;
    [field: SerializeField] private float forceDown { get; set; } = 10f;


    private float horizontalMovement { get; set; }
    private float verticalMovement { get; set; }

    private Vector2 movementInput { get; set; }
    private Vector3 moveDirection { get; set; }
    private Vector3 slopeMoveDirection { get; set; }

    [field: Header("Debug")]
    // Private ground properties
    [field: SerializeField, ReadOnlyField] private bool isGrounded { get; set; }
    private float groundDistance { get; set; } = 0.4f;

    // Private slopes properties
    RaycastHit slopeHit;

    void Start() {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        playerHeight = playerCollider.height;
        halfPlayerHeight = playerHeight / 2f;
    }

    void Update() {
        if (!LevelManager.IsLevelOnGoing)
            return;

        ControlDrag();
        ControlSpeed();
        IsGrounded();
    }

    void FixedUpdate() {
        if (!LevelManager.IsLevelOnGoing)
            return;

        MovePlayer();

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void OnMovement(InputValue value) {
        movementInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value) {
        isSprinting = value.isPressed;
    }

    void OnJump() {
        if (!LevelManager.IsLevelOnGoing)
            return;

        if (isGrounded)
            Jump();
    }

    void MovePlayer() {
        horizontalMovement = movementInput.x;
        verticalMovement = movementInput.y;

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

        if (isGrounded && !OnSlope()) {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope()) {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded) {
            if (rb.velocity.y < 0 && rb.useGravity) {
                rb.AddForce(Vector3.down * forceDown, ForceMode.Force);
            }

            rb.AddForce(moveDirection.normalized * moveSpeed * airMovementMultiplier, ForceMode.Acceleration);
        }
    }

    void ControlDrag() {
        if (isGrounded) {
            rb.drag = groundDrag;
        } else {
            rb.drag = airDrag;
        }
    }

    void ControlSpeed() {
        if (isSprinting && isGrounded) {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else if (playerWallRun.WallLeft || playerWallRun.WallRight) {
            moveSpeed = sprintSpeed * 1.5f;
        }
        else {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    bool IsGrounded() {
        //return isGrounded = Physics.Raycast(transform.position, Vector3.down, halfPlayerHeight + 0.1f, groundMask);
        return isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void Jump() {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset the y velocity
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, halfPlayerHeight + 0.5f)) {
            if (slopeHit.normal != Vector3.up) {
                return true;
            }
        }
        return false;
    }
}
