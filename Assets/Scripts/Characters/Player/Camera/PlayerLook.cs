using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour {

    [field: Header("- Autoattach propierties -")]
    [field: SerializeField, GetComponent, ReadOnlyField] private PlayerWallRun playerWallRun { get; set; }

    [field: Header("Camera settings")]
    [field: SerializeField] private Transform cam { get; set; }
    [field: SerializeField] private Transform orientation { get; set; }
    [field: SerializeField] private float sensX { get; set; } = 10f;
    [field: SerializeField] private float sensY { get; set; } = 10f;
    [field: SerializeField] private float multiplier { get; set; } = 0.01f;
   
    private float mouseX { get; set; }
    private float mouseY { get; set; }

    private float xRotation { get; set; }
    private float yRotation { get; set; }

    Vector2 cameraInput { get; set; } = Vector2.zero;

    void Update() {
        if (!LevelManager.IsLevelOnGoing || PauseManager.onPause)
            return;

        PlayerRotation();
    }

    void LateUpdate() {
        if (!LevelManager.IsLevelOnGoing || PauseManager.onPause)
            return;

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

        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, playerWallRun.Tilt);
        orientation.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    void PlayerRotation() {
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
