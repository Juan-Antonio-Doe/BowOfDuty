using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour {

	[field: Header("NPC settings")]
    [field: SerializeField] protected float maxHealth { get; set; } = 20f;
    [field: SerializeField, ReadOnlyField] protected float health { get; set; } = 20f;

    [field: SerializeField] protected float attackRange { get; set; } = 10f;
    public float AttackRange { get => attackRange; }
    [field: SerializeField] protected float attackCooldown { get; set; } = 1f;
    public float AttackCooldown { get => attackCooldown; }

    protected bool isDead { get; set; }
    public bool IsDead { get => isDead; }

    public virtual void TakeDamage(float damage) {
        if (health > 0) {
            health -= damage;
            if (health <= 0) {
                health = 0;
                Die();
            }
            //UpdateUI();
        }
    }

    protected virtual void Die() {
        isDead = true;
        gameObject.SetActive(false);
    }
}
