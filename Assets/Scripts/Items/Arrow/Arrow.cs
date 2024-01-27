using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private Rigidbody rb { get; set; }

    [field: Header("Arrow settings")]
    [field: SerializeField] private BoxCollider childColl { get; set; }
    [field: SerializeField] public float damage { get; set; } = 10f;
    [field: SerializeField, ReadOnlyField] private bool stucked { get; set; } 

    private void OnTriggerEnter(Collider other) {
        if (stucked) return;

        if (other.CompareTag("Untagged")) {
            StickOnPoint(other.transform);
        }

        // Compare if the target is an enemy and the arrow is an ally.
        if (other.CompareTag("Enemy") && gameObject.layer == 10) {
            StickOnPoint(other.transform);
            other.GetComponent<EnemyArcher>().TakeDamage(damage);
            return;
        }

        // Compare if the target is a Ghost and the arrow is an ally.
        if (other.CompareTag("Ghost") && gameObject.layer == 10) {
            StickOnPoint(other.transform);
            other.GetComponent<Ghost>().TakeDamage(damage);
            return;
        }

        // Compare if the target is a Tower and the arrow is an ally.
        if (other.CompareTag("Tower") && gameObject.layer == 10) {
            StickOnPoint(other.transform);
            other.GetComponent<TowerColliderProxy>().TakeDamage(damage);
            return;
        }

        // Compare if the target is a Battering Ram and the arrow is an ally.
        if (other.CompareTag("BatteringRam") && gameObject.layer == 10) {
            StickOnPoint(other.transform);
            other.GetComponent<BatteringRamColliderProxy>().TakeDamage(damage);
            return;
        }

        // Compare if the target is a Battering Ram and the arrow is an enemy.
        if (other.CompareTag("BatteringRam") && gameObject.layer == 11) {
            StickOnPoint(other.transform);
            other.GetComponent<BatteringRamColliderProxy>().TakeDamage(damage);
            return;
        }

        // Compare if the taget is a Tower and the arrow is an enemy.
        if (other.CompareTag("Tower") && gameObject.layer == 11) {
            StickOnPoint(other.transform);
            other.GetComponent<TowerColliderProxy>().TakeDamage(damage);
            return;
        }

        // Compare if the target is an ally and the arrow is an enemy.
        if (other.CompareTag("Ally") && gameObject.layer == 11) {
            StickOnPoint(other.transform);
            other.GetComponent<AllyArcher>().TakeDamage(damage);
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