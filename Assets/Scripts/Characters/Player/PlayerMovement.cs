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

    [field: Header("Player stats")]
    [field: SerializeField, ReadOnlyField] private float playerHeight { get; set; } = 2f;
    private float halfPlayerHeight { get; set; }
    // groundMask by default is the 0 and 6 layers (Default and Ground)
    [field: SerializeField] private LayerMask groundMask { get; set; } = 1 << 0 | 1 << 6;

    [field: Header("Movement settings")]
    [field: SerializeField] private float moveSpeed { get; set; } = 6f;
    [field: SerializeField] private float movementMultiplier { get; set; } = 10f;
    [field: SerializeField] private float groundDrag { get; set; } = 6f;

    [field: Header("Jump settings")]
    [field: SerializeField] private float airMovementMultiplier { get; set; } = 2.35f;
    [field: SerializeField] private float jumpForce { get; set; } = 5f;
    [field: SerializeField] private float airDrag { get; set; } = 2f;


    private float horizontalMovement { get; set; }
    private float verticalMovement { get; set; }

    private Vector2 movementInput { get; set; }
    private Vector3 moveDirection { get; set; }

    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private bool isGrounded { get; set; }

    void Start() {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        playerHeight = playerCollider.height;
        halfPlayerHeight = playerHeight / 2f;
    }

    void Update() {
        ControlDrag();
        IsGrounded();
    }

    void FixedUpdate() {
        MovePlayer();
    }

    void OnMovement(InputValue value) {
        movementInput = value.Get<Vector2>();
    }

    void OnJump() {
        if (isGrounded)
            Jump();
    }

    void MovePlayer() {
        horizontalMovement = movementInput.x;
        verticalMovement = movementInput.y;

        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;

        if (isGrounded) {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        } 
        else if (!isGrounded) {
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

    bool IsGrounded() {
        return isGrounded = Physics.Raycast(transform.position, Vector3.down, halfPlayerHeight + 0.1f, groundMask);
    }

    void Jump() {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
}
