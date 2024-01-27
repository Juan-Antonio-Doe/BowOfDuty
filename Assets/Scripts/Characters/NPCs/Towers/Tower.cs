using Nrjwolf.Tools.AttachAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class Tower : NPC {

    [field: Header("Autoattach properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] public EnemiesManager enemies { get; private set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] public AlliesManager allies { get; private set; }
    [field: SerializeField, GetComponent, ReadOnlyField] public TowerBow bow { get; private set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private BoxCollider bigCollider { get; set; }
    [field: SerializeField, ReadOnlyField] private TowerColliderProxy[] towerPieces { get; set; }

    [field: Header("Tower Attack")]    
    [field: SerializeField] public bool isEnemyTower { get; private set; }
    [field: SerializeField] private LayerMask targetMasks { get; set; }
    [field: SerializeField] private LayerMask obstacleMask { get; set; }

    [field: Header("Tower UI")]
    [field: SerializeField] private Image healthBar { get; set; }
    [field: SerializeField, ReadOnlyField] private Text healthText { get; set; }
    [field: SerializeField, ReadOnlyField] private GameObject healthCanvasGO { get; set; }
    [field: SerializeField] private bool revalidateProperties { get; set; }
    [field: SerializeField] private float canvasDisplayTime { get; set; } = 2f;
    private bool isPlayerLookingAtMe { get; set; }
    private bool isHideCanvasCoActive { get; set; }
    private Coroutine hideCanvasAfterTimeCo { get; set; }

    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private Transform attackTarget { get; set; }
    public Transform AttackTarget { get => attackTarget; set => attackTarget = value; }

    private bool isVisible { get; set; } = true;

    void OnValidate() {
#if UNITY_EDITOR
        UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
        if (!isValidPrefabStage && prefabConnected) {
            // Variables that will only be checked when they are in a scene
            if (!Application.isPlaying) {
                if (revalidateProperties)
                    ValidateAssings();
            }
        }
#endif
    }

    void ValidateAssings() {
        if (healthText == null || revalidateProperties) {
            healthText = healthBar.transform.GetChild(0).GetComponent<Text>();
        }

        if (healthCanvasGO == null || revalidateProperties) {
            healthCanvasGO = healthBar.gameObject.GetComponentInParent<Canvas>().gameObject;
        }

        if (towerPieces == null || towerPieces.Length == 0 || revalidateProperties) {
            towerPieces = GetComponentsInChildren<TowerColliderProxy>();
        }

        revalidateProperties = false;
    }

    void OnEnable() {
        // Suscribirse al evento cuando el objeto se activa
        PlayerManager.OnPlayerLookAtEnemy += ShowHideCanvas;
    }

    void OnDisable() {
        // Anular la suscripción al evento cuando el objeto se desactiva
        PlayerManager.OnPlayerLookAtEnemy -= ShowHideCanvas;
    }

    void Start() {
        health = maxHealth;
        UpdateUI();

        healthCanvasGO.SetActive(false);
    }

    void Update() {
        if (!LevelManager.IsLevelOnGoing)
            return;

        if (!isDead) {
            if (attackTarget != null) {
                if (!attackTarget.gameObject.activeInHierarchy) {
                    attackTarget = null;
                }
            }

            if (attackTarget == null) {
                CheckNearby();
            }

            if (attackTarget != null) {
                Attack();
            }
            else {
                if (!bow.CanShoot()) {
                    bow.CancelAttack();
                }
            }
        }

        if (isEnemyTower) {
            if (!isPlayerLookingAtMe && healthCanvasGO.activeInHierarchy && !isHideCanvasCoActive) {
                hideCanvasAfterTimeCo = StartCoroutine(HideCanvasAfterTimeCo(canvasDisplayTime));
            }
        }
        
    }

    private void Attack() {
        AimTarget(attackTarget);

        if (bow.CanShoot()) {
            bow.Shoot();
        }
    }

    void CheckNearby() {
        // Detect the firt ally NPC or Player that is on attack range using OverlapSphere.
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, targetMasks);
        attackTarget = null; // Reset the attack target

        foreach (Collider collider in colliders) {
            if (collider.gameObject != gameObject && collider.gameObject.activeInHierarchy) {
                // Check if there is an obstacle between the enemy and the target
                Transform origin = bow.arrowSpawnPoint;
                if (Physics.Raycast(origin.position, collider.gameObject.transform.position - origin.position, out RaycastHit hit, attackRange, obstacleMask)) {
                    //Debug.Log($"Tower Obstacle: {hit.collider.gameObject.name}");
                    continue; // Skip this iteration if there is an obstacle between the enemy and the target
                }
                else {
                    attackTarget = collider.gameObject.transform;
                    return; // Stop the loop once the first valid target is found
                }
            }
        }
    }

    void AimTarget(Transform attackTarget) {
        bow.arrowSpawnPoint.LookAt(attackTarget);
    }

    public override void TakeDamage(float damage) {
        if (health > 0) {
            health -= damage;
            if (health <= 0) {
                health = 0;
                Die();
            }
            UpdateUI();
        }

        if (!isEnemyTower && isVisible && !isDead) {
            healthCanvasGO.SetActive(true);
        }
    }

    protected override void Die() {
        isDead = true;
        gameObject.layer = 3;
        bigCollider.enabled = false;

        if (hideCanvasAfterTimeCo != null) {
            StopCoroutine(hideCanvasAfterTimeCo);
            isHideCanvasCoActive = false;
        }

        if (healthCanvasGO.activeInHierarchy) {
            healthCanvasGO.SetActive(false);
        }

        StartCoroutine(ApplyExplosionForceCo());
    }

    void UpdateUI() {
        healthBar.fillAmount = health / maxHealth;
        healthText.text = $"{health} / {maxHealth}";
    }

    void ShowHideCanvas(Transform enemyTransform) {

        // Si el jugador está mirando a este enemigo y la vida del enemigo no está completa
        if (enemyTransform == transform && health < maxHealth) {
            // Activar el canvas de salud
            healthCanvasGO.SetActive(true);
            // Indicar que el jugador está mirando al enemigo
            isPlayerLookingAtMe = true;
        }

        isPlayerLookingAtMe = false;
    }

    IEnumerator HideCanvasAfterTimeCo(float time) {
        isHideCanvasCoActive = true;

        // Esperar el tiempo especificado
        yield return new WaitForSeconds(time);

        // Desactivar el canvas de salud
        healthCanvasGO.SetActive(false);
        isHideCanvasCoActive = false;
    }

    IEnumerator ApplyExplosionForceCo() {
        for (int i = 0; i < towerPieces.Length; i++) {
            towerPieces[i].ApplyExplosionForce();
            yield return null;
        }

        yield return new WaitForSeconds(12f);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected() {
        // Draw the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw the shoot direction
        if (attackTarget != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(bow.arrowSpawnPoint.position, attackTarget.position);
        }
    }

    private void OnBecameInvisible() {
        if (!Application.isPlaying)
            return;

        isVisible = false;

        if (healthCanvasGO.activeInHierarchy) {
            healthCanvasGO.SetActive(false);

            if (hideCanvasAfterTimeCo != null) {
                StopCoroutine(hideCanvasAfterTimeCo);
                isHideCanvasCoActive = false;
            }
        }
    }

    private void OnBecameVisible() {
        if (!Application.isPlaying)
            return;

        isVisible = true;

        if (!isEnemyTower && health < maxHealth && !isDead) {
            healthCanvasGO.SetActive(true);
        }
    }
}