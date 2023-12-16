using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    [field: Header("Player settings")]
    [field: SerializeField] private float health { get; set; } = 100f;
    [field: SerializeField] private float maxHealth { get; set; } = 100f;

    public void TakeDamage(float damage) {
        if (health > 0) {
            health -= damage;
            if (health <= 0) {
                Die();
            }
        }
    }

    private void Die() {
        
    }
}
