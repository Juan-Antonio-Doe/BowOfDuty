using Nrjwolf.Tools.AttachAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {

	[field: Header("Enemy settings")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] public EnemiesManager enemies { get; private set; }
    [field: SerializeField] protected float maxHealth { get; set; } = 20f;
	[field: SerializeField, ReadOnlyField] protected float health { get; set; } = 20f;
	//[field: SerializeField] protected float damage { get; set; }
	[field: SerializeField] protected float attackRange { get; set; } = 10f;
    public float AttackRange { get => attackRange; }
	[field: SerializeField] protected float attackCooldown { get; set; } = 1f;

	public virtual void TakeDamage(float damage) {
        if (health > 0) {
            health -= damage;
            if (health <= 0) {
                Die();
            }
        }
    }

    protected virtual void Die() {
        gameObject.SetActive(false);
    }
}