using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBow : MonoBehaviour {
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public float shootForce = 20f;
    private GameObject currentArrow;
    private bool isDrawing = false;
    private float drawStartTime;

    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            StartDrawingBow();
        }
        else if (isDrawing && Input.GetButtonUp("Fire1")) {
            ReleaseArrow();
        }
    }

    void StartDrawingBow() {
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        currentArrow.transform.parent = arrowSpawnPoint;
        drawStartTime = Time.time;
        isDrawing = true;
    }

    void ReleaseArrow() {
        float drawDuration = Time.time - drawStartTime;
        float actualShootForce = Mathf.Clamp(shootForce * drawDuration, 0, shootForce);
        currentArrow.transform.parent = null;
        Rigidbody arrowRigidbody = currentArrow.GetComponent<Rigidbody>();
        arrowRigidbody.isKinematic = false;
        arrowRigidbody.AddForce(arrowSpawnPoint.forward * actualShootForce, ForceMode.Impulse);
        isDrawing = false;
    }
}

