using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    [field: Header("Player settings")]
    [field: SerializeField, ReadOnlyField] private float health { get; set; } = 100f;
    [field: SerializeField] private float maxHealth { get; set; } = 100f;
    [field: SerializeField] private float enemyRayCastDistance { get; set; } = 60f;
    [field: SerializeField] private LayerMask enemyLayer { get; set; } = 1 << 8;

    private bool isDead { get; set; }
    public bool IsDead { get => isDead; }

    // Definir un evento que se dispara cuando el jugador mira a un enemigo
    public static event Action<Transform> OnPlayerLookAtEnemy;

    private float centerWidth { get; set; }
    private float centerHeight { get; set; }

    void Start() {
        centerWidth = Screen.width / 2;
        centerHeight = Screen.height / 2;

        health = maxHealth;
    }

    void Update() {
        if (!LevelManager.IsLevelOnGoing)
            return;

        if (!isDead) {
            CheckEnemyInFront();
        }
    }

    public void TakeDamage(float damage) {
        if (health > 0) {
            health -= damage;
            if (health <= 0) {
                Die();
            }
        }

        Debug.Log($"Player health: <color=green>{health}</color>");
    }

    private void Die() {
        isDead = true;
    }

    void CheckEnemyInFront() {
        // Crear un rayo desde el centro de la cámara
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(centerWidth, centerHeight, 0));

        // Crear una variable para almacenar la información del raycast
        RaycastHit hit;

        // Realizar el raycast
        if (Physics.Raycast(ray, out hit, enemyRayCastDistance, enemyLayer)) {
            // Si el raycast golpea a un enemigo, disparar el evento
            OnPlayerLookAtEnemy?.Invoke(hit.transform);
        }
    }
}
