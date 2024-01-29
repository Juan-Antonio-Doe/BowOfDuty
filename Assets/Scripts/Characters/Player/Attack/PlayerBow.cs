using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBow : Bow {

    [field: Header("Player Shoot settings")]
    /*[field: SerializeField] private Rigidbody arrowPrefab { get; set; }
    [field: SerializeField] private Transform arrowSpawnPoint { get; set; }
    [field: SerializeField] private float maxShootForce { get; set; } = 20f;*/
    [field: SerializeField] private float fireRate { get; set; } = 0.5f;
    private float nextFire { get; set; }
    [field: SerializeField] private Transform playerCamera { get; set; }

    /*private Rigidbody currentArrow { get; set;}
    private bool isDrawing { get; set; }*/
    private float drawStartTime { get; set; }

    void Update() {
        if (!LevelManager.IsLevelOnGoing || PauseManager.onPause)
            return;

        if (Input.GetButtonDown("Fire1") && Time.time > nextFire) {
            nextFire = Time.time + fireRate;
            StartDrawingBow();
        }
        else if (isDrawing && Input.GetButtonUp("Fire1")) {
            ReleaseArrow();
        }

        if (isDrawing) {
            currentArrow.transform.rotation = Quaternion.LookRotation(playerCamera.transform.forward);
        }
    }

    protected override void StartDrawingBow() {
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, playerCamera.transform.rotation);
        currentArrow.transform.parent = arrowSpawnPoint;
        drawStartTime = Time.time;
        isDrawing = true;
    }

    void ReleaseArrow() {
        float drawDuration = Time.time - drawStartTime;
        float actualShootForce = Mathf.Clamp(maxShootForce * drawDuration, 0, maxShootForce);
        currentArrow.transform.parent = null;
        //Rigidbody arrowRigidbody = currentArrow.GetComponent<Rigidbody>();
        Rigidbody arrowRigidbody = currentArrow.rb;
        arrowRigidbody.isKinematic = false;
        //arrowRigidbody.AddForce(arrowSpawnPoint.forward * actualShootForce, ForceMode.Impulse);
        arrowRigidbody.AddForce(playerCamera.transform.forward * actualShootForce, ForceMode.Impulse);
        currentArrow.PlayFlySound();
        //arrowRigidbody.AddTorque(playerCamera.transform.forward * actualShootForce, ForceMode.Impulse);
        arrowRigidbody.useGravity = true;
        isDrawing = false;
        Destroy(currentArrow.gameObject, 5f);
    }
}

