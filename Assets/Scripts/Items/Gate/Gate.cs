using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {

    [field: Header("Gate properties")]
    [field: SerializeField] public static int maxHealth { get; private set; } = 6;
    [field: SerializeField, ReadOnlyField] public int health { get; set; }
    [field: SerializeField] private float damageCooldown { get; set; } = 8f;

    private bool isDead { get; set; }
	private bool inCooldown { get; set; }

    void Start() {
        health = maxHealth;
    }

    public void TakeDamage(int damage) {
        if (health > 0) {
            health -= damage;
            if (health <= 0) {
                health = 0;
                Die();
            }
        }
    }

    void Die() {
        isDead = true;
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("BatteringRam")) {
            if (!inCooldown) {
                TakeDamage(1);
                if (!isDead)
                    StartCoroutine(DamageCooldownCo());
            }
        }
    }

    IEnumerator DamageCooldownCo() {
        inCooldown = true;
        yield return new WaitForSeconds(damageCooldown);
        inCooldown = false;
    }
}