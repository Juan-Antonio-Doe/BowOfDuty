using Nrjwolf.Tools.AttachAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour {

    [field: Header("- Autoattach propierties -")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private LevelManager levelManager { get; set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private PlayerMovement playerMovement { get; set; }
    [field: SerializeField, ReadOnlyField] private Transform deathRespawnPoint { get; set; }
    [field: SerializeField, ReadOnlyField] private Transform playerRespawnPoint { get; set; }
    [field: SerializeField] private bool revalidateProperties { get; set; } = false;

    [field: Header("Player settings")]
    [field: SerializeField, ReadOnlyField] private float health { get; set; } = 100f;
    [field: SerializeField] private float maxHealth { get; set; } = 100f;
    [field: SerializeField, ReadOnlyField] private int lives { get; set; }
    [field: SerializeField] private int maxLives { get; set; } = 3;
    [field: SerializeField] private float enemyRayCastDistance { get; set; } = 60f;
    [field: SerializeField] private LayerMask enemyLayer { get; set; } = 1 << 8;

    [field: Header("Death")]
    [field: SerializeField] private float maxGhostTime { get; set; } = 30f;
    [field: SerializeField, ReadOnlyField] private float ghostTime { get; set; }
    [field: SerializeField] private UnityEvent onDeadEvent { get; set; }

    private bool isDead { get; set; }
    public bool IsDead { get => isDead; }
    private bool isGhost { get; set; }

    // Definir un evento que se dispara cuando el jugador mira a un enemigo
    public static event Action<Transform> OnPlayerLookAtEnemy;

    private float centerWidth { get; set; }
    private float centerHeight { get; set; }

    void OnValidate() {
#if UNITY_EDITOR
        UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
        if (!isValidPrefabStage && prefabConnected) {
            // Variables that will only be checked when they are in a scene
            if (!Application.isPlaying) {
                if (revalidateProperties)
                    ValidateProperties();
            }
        }

#endif
    }

    void ValidateProperties() {
        if (deathRespawnPoint == null || revalidateProperties) {
            deathRespawnPoint = GameObject.FindGameObjectWithTag("DeathRespawnPoint").transform;
        }
        if (playerRespawnPoint == null || revalidateProperties) {
            playerRespawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
        }

        revalidateProperties = false;
    }


    void Start() {
        centerWidth = Screen.width / 2;
        centerHeight = Screen.height / 2;

        health = maxHealth;
        lives = maxLives;
        ghostTime = maxGhostTime;
    }

    void Update() {
        if (!LevelManager.IsLevelOnGoing)
            return;

        if (!isDead) {
            CheckEnemyInFront();
        }

        if (isGhost) {
            ghostTime -= Time.deltaTime;

            if (ghostTime <= 0) {
                ghostTime = 0;
                lives--;
                if (lives == 0) {
                    levelManager.EndLevel(false);
                } else {
                    ResurrectPlayer();
                }
            }
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

    void Die() {
        isDead = true;
        isGhost = true;
        playerMovement.unlimitedSprint = true;
        TeleportToDeathPrision();
        onDeadEvent?.Invoke();
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

    void TeleportToDeathPrision() {
        // ToDo: Apply death shader.
        transform.position = deathRespawnPoint.position;
    }

    public void ResurrectPlayer() {
        isGhost = false;
        transform.position = playerRespawnPoint.position;
        health = maxHealth;
        isDead = false;
        ghostTime = maxGhostTime;
        playerMovement.unlimitedSprint = false;
    }
}
