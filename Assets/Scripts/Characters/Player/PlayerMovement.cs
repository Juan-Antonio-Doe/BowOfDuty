using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the player's movement.
/// Uses a Rigidbody component to move the player.
/// </summary>

public class PlayerMovement : MonoBehaviour {

    [field: Header("Movement settings")]
    [field: SerializeField] private float _speed { get; set; } = 5f;
    [field: SerializeField] private float _jumpForce { get; set; } = 10f;
    [field: SerializeField] private Transform _groundCheck { get; set; }
    [field: SerializeField] private LayerMask _groundLayer { get; set; }
    [field: SerializeField] private Rigidbody _rb { get; set; }
    [field: SerializeField] private Animator _animator { get; set; }
    [field: SerializeField] private PlayerInputActions _inputActions { get; set; }

    private Vector2 _movementInput { get; set; }
    [field: ReadOnlyField] private bool _isGrounded { get; set; }

    private bool lockMovement { get; set; } = false;

    void Start() {
        if (_rb == null)
            _rb = GetComponent<Rigidbody>();

    }

    void Update() {

        // Check if the player is grounded
        _isGrounded = Physics.CheckSphere(_groundCheck.position, 0.1f, _groundLayer);

        // Jump using the button assigned in the Input Manager
        if (_inputActions.Player.Jump.triggered && _isGrounded && !lockMovement) {
            Jump();
        }
    }

    void FixedUpdate() {
        // Mover al jugador
        Move();
    }

    void Move() {
        Vector3 movement = new Vector3(_movementInput.x, 0f, _movementInput.y);
        Vector3 moveDirection = transform.TransformDirection(movement);

        _rb.AddForce(moveDirection * _speed, ForceMode.VelocityChange);
    }

    void Jump() {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    public void OnMove(InputValue value) {
        _movementInput = value.Get<Vector2>();
    }
}
