using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

    [field: Header("Autoattach propierties")]

    [field: Header("Camera settings")]
    [field: SerializeField] private Transform cameraPosition { get; set; }

    void Update() {
        transform.position = cameraPosition.position;
    }
}
