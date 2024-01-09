using Nrjwolf.Tools.AttachAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : NPC {

	[field: Header("Enemy settings")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] public EnemiesManager enemies { get; private set; }

    [field: SerializeField] protected Image healthBar { get; set; }
    [field: SerializeField, ReadOnlyField] protected Text healthText { get; set; }
    [field: SerializeField] protected GameObject enemyCanvasGO { get; set; }
    [field: SerializeField] protected float canvasDisplayTime { get; set; } = 2f;
    protected bool isPlayerLookingAtEnemy { get; set; }
    protected bool isHideCanvasCoActive { get; set; }

    public override void TakeDamage(float damage) {
        if (health > 0) {
            health -= damage;
            if (health <= 0) {
                health = 0;
                Die();
            }
            UpdateUI();
        }
    }

    void UpdateUI() {
        healthBar.fillAmount = health / maxHealth;
        healthText.text = $"{health} / {maxHealth}";
    }
}