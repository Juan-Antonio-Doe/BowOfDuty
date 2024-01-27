using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerColliderProxy : MonoBehaviour {

	[field: SerializeField, GetComponent, ReadOnlyField] private Rigidbody rb { get; set; }
	[field: SerializeField] private Tower tower { get; set; }

	public void TakeDamage(float damage) {
        tower.TakeDamage(damage);
    }

	public void ApplyExplosionForce() {
		rb.isKinematic = false;
		// Random explosion direction
		rb.AddExplosionForce(1000f, gameObject.transform.position, 100f, Random.Range(-5f, 5f), ForceMode.Impulse);
	}
}