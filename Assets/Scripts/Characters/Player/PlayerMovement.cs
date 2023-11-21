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

    [field: Header("Movement settings")]
    [field: SerializeField] private float moveSpeed { get; set; } = 6f;
    [field: SerializeField] private float movementMultiplier { get; set; } = 10f;

    private float horizontalMovement { get; set; }
    private float verticalMovement { get; set; }

    private float rbDrag { get; set; } = 6f;

    private Vector3 moveDirection { get; set; }

    void Start() {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;
    }

    void Update() {
        ControlDrag();
    }

    void FixedUpdate() {
        MovePlayer();
    }

    void OnMovement(InputValue value) {
        Vector2 inputVector = value.Get<Vector2>();
        horizontalMovement = inputVector.x;
        verticalMovement = inputVector.y;

        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
        Debug.Log($"Move direction: {moveDirection}");
    }

    void MovePlayer() {
        rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
    }

    void ControlDrag() {
        rb.drag = rbDrag;
    }
}
