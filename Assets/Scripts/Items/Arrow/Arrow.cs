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

        // Compare if the arrow is an ally and the target is an enemy.
        if (other.CompareTag("Enemy") && gameObject.layer == 10) {
            StickOnPoint(other.transform);
            other.GetComponent<EnemyArcher>().TakeDamage(damage);
            return;
        }

        if (other.CompareTag("Player") && gameObject.layer == 11) {
            StickOnPoint(other.transform);
            other.GetComponent<PlayerManager>().TakeDamage(damage);
            return;
        }

        if ((other.CompareTag("PlayerBase") && gameObject.layer == 11) || 
            (other.CompareTag("EnemyBase") && gameObject.layer == 10)) {

            StickOnPoint(other.transform);
            other.GetComponent<BaseHealthAlpha>().TakeDamage(damage);
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