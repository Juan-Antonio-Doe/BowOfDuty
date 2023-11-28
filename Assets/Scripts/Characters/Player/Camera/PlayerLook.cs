using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour {

    [field: Header("- Autoattach propierties -")]
    [field: SerializeField, GetComponentInChildren, ReadOnlyField] private Camera cam { get; set; }

    [field: Header("Camera settings")]
    [field: SerializeField] private float sensX { get; set; } = 10f;
    [field: SerializeField] private float sensY { get; set; } = 10f;
    [field: SerializeField] private float multiplier { get; set; } = 0.01f;
   
    private float mouseX { get; set; }
    private float mouseY { get; set; }

    private float xRotation { get; set; }
    private float yRotation { get; set; }

    Vector2 cameraInput { get; set; } = Vector2.zero;
	
    void Start() {
        if (cam == null)
            cam = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        PlayerRotation();
    }

    void LateUpdate() {
        CameraRotation();
    }

    void OnCameraMove(InputValue value) {
        cameraInput = value.Get<Vector2>();
    }

    void CameraRotation() {
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void PlayerRotation() {
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
