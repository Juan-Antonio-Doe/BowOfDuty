using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private Rigidbody rb { get; set; }

    [field: Header("Arrow settings")]
    [field: SerializeField] public float damage { get; set; } = 10f;
    [field: SerializeField, ReadOnlyField] private bool stucked { get; set; } 

    private void OnTriggerEnter(Collider other) {
        if (stucked) return;

        if (other.CompareTag("Untagged")) {
            StickOnPoint(other.transform);
        }

        if (other.CompareTag("Enemy")) {
            StickOnPoint(other.transform);
            other.GetComponent<EnemyArcher>().TakeDamage(damage);
            return;
        }

        if (other.CompareTag("Player")) {
            StickOnPoint(other.transform);
            other.GetComponent<PlayerManager>().TakeDamage(damage);
            return;
        }
    }

    private void StickOnPoint(Transform parent) {
        stucked = true;
        rb.isKinematic = true;
        rb.useGravity = false;
        transform.SetParent(parent);
    }
}