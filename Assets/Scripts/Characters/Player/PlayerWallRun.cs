using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWallRun : MonoBehaviour {

    [field: Header("- Autoattach propierties -")]
    [field: SerializeField, GetComponent, ReadOnlyField] private Rigidbody rb { get; set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private PlayerMovement playerMovement { get; set; }

    [field: Header("Wall movement")]
    [field: SerializeField] private Transform orientation { get; set; }
    [field: SerializeField] private LayerMask wallWalkableMask { get; set; } = 1 << 0 | 1 << 6;

    [field: Header("Camera effect settings")]
    [field: SerializeField] private Camera cam { get; set; }
    private float fov { get; set; }
    [field: SerializeField] private float wallRunFov { get; set; } = 110f;
    [field: SerializeField] private float wallRunFovTime { get; set; } = 20f;
    [field: SerializeField] private float camTilt { get; set; }
    [field: SerializeField] private float camTiltTime { get; set; }

    private float tilt { get; set; }
    public float Tilt { get => tilt; }

    [field: Header("Wall detection")]
    [field: SerializeField] private float wallRunDistance { get; set; } = 0.6f;
    [field: SerializeField] private float minimumJumpHeight { get; set; } = 1.5f;

    [field: Header("Wall running")]
    [field: SerializeField] private float wallRunGravity { get; set; } = 1f;
    [field: SerializeField] private float wallRunJumpForce { get; set; } = 6f;

    private bool wallLeft { get; set; }
    public bool WallLeft { get => wallLeft; }
    private bool wallRight { get; set; }
    public bool WallRight { get => wallRight; }

    private bool jumpPressed { get; set; }

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    void Start() {
        fov = cam.fieldOfView;
    }

    void Update() {
        CheckWall();

        if (CanWallRun()) {
            if (wallLeft) {
                StartWallRun();
                //rb.AddForce(orientation.right * 50f);
            } 
            else if (wallRight) {
                StartWallRun();
                //rb.AddForce(-orientation.right * 50f);
            }
            else {
                StopWallRun();
                //rb.AddForce(-orientation.right * 50f);
            }
        }
    }

    void CheckWall() {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallRunDistance, wallWalkableMask);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallRunDistance, wallWalkableMask);
    }

    bool CanWallRun() {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    void StartWallRun() {
        rb.useGravity = false;

        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        if (wallLeft)
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        else if (wallRight)
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunFov, wallRunFovTime * Time.deltaTime);
    }

    void StopWallRun() {
        rb.useGravity = true;

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallRunFovTime * Time.deltaTime);

        tilt = Mathf.Lerp(tilt, 0f, camTiltTime * Time.deltaTime);
    }

    void OnJump() {
        if (wallLeft) {
            Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100f, ForceMode.Force);
        }
        else if (wallRight) {
            Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100f, ForceMode.Force);
        }
    }
}
