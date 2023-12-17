using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBow : MonoBehaviour {

    [field: Header("Shoot settings")]
    [field: SerializeField] private Rigidbody arrowPrefab { get; set; }
    [field: SerializeField] private Transform arrowSpawnPoint { get; set; }
    [field: SerializeField] private float shootForce { get; set; } = 20f;
    [field: SerializeField] private Camera playerCamera { get; set; }

    private Rigidbody currentArrow { get; set;}
    private bool isDrawing { get; set; }
    private float drawStartTime { get; set; }

    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            StartDrawingBow();
        }
        else if (isDrawing && Input.GetButtonUp("Fire1")) {
            ReleaseArrow();
        }

        if (isDrawing) {
            currentArrow.transform.rotation = Quaternion.LookRotation(playerCamera.transform.forward);
        }
    }

    void StartDrawingBow() {
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, playerCamera.transform.rotation);
        currentArrow.transform.parent = arrowSpawnPoint;
        drawStartTime = Time.time;
        isDrawing = true;
    }

    void ReleaseArrow() {
        float drawDuration = Time.time - drawStartTime;
        float actualShootForce = Mathf.Clamp(shootForce * drawDuration, 0, shootForce);
        currentArrow.transform.parent = null;
        //Rigidbody arrowRigidbody = currentArrow.GetComponent<Rigidbody>();
        Rigidbody arrowRigidbody = currentArrow;
        arrowRigidbody.isKinematic = false;
        //arrowRigidbody.AddForce(arrowSpawnPoint.forward * actualShootForce, ForceMode.Impulse);
        arrowRigidbody.AddForce(playerCamera.transform.forward * actualShootForce, ForceMode.Impulse);
        //arrowRigidbody.AddTorque(playerCamera.transform.forward * actualShootForce, ForceMode.Impulse);
        arrowRigidbody.useGravity = true;
        isDrawing = false;
        Destroy(currentArrow.gameObject, 5f);
    }
}

