using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyArcher : Enemy {

    [field: Header("Autoattach properties")]
    /*[field: SerializeField, FindObjectOfType, ReadOnlyField] private EnemiesManager enemiesManager { get; set; }
    public EnemiesManager EnemiesManager { get { return enemiesManager; } }*/
    [field: SerializeField, GetComponent, ReadOnlyField] protected NavMeshAgent agent { get; set; }
    [field: SerializeField, ReadOnlyField] protected Transform playerBase { get; set; }
    public Transform PlayerBase { get => playerBase; }
    [field: SerializeField, GetComponent, ReadOnlyField] protected EnemyBow bow { get; set; }
    public EnemyBow Bow { get => bow; }
    [field: SerializeField] private bool revalidateProperties { get; set; }

    [field: Header("Move settings")]
    [field: SerializeField] private float moveSpeed { get; set; } = 4f;
    public float MoveSpeed { get => moveSpeed; }

    [field: Header("Attack settings")]
    [field: SerializeField] private LayerMask targetMasks { get; set; }
    [field: SerializeField] private LayerMask obstacleMask { get; set; }

    private EnemyArcherState currentState { get; set; }

    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private Transform attackTarget { get; set; }
    public Transform AttackTarget { get => attackTarget; set => attackTarget = value; }
    [field: SerializeField, ReadOnlyField] private EnemyArcherState.STATE meStateNow { get; set; }

    private Coroutine hideCanvasAfterTimeCo;
    private bool isStarted { get; set; }

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
        /*if (playerBase == null || revalidateProperties) {
            playerBase = GameObject.FindGameObjectWithTag("PlayerBase").transform.GetChild(0);
        }*/

        if (healthText == null || revalidateProperties) {
            healthText = healthBar.transform.GetChild(0).GetComponent<Text>();
        }

        if (enemyCanvasGO == null || revalidateProperties) {
            enemyCanvasGO = healthBar.gameObject.GetComponentInParent<Canvas>().gameObject;
        }

        revalidateProperties = false;
    }

    void OnEnable() {
        // Suscribirse al evento cuando el objeto se activa
        PlayerManager.OnPlayerLookAtEnemy += ShowHideCanvas;

        if (!isStarted)
            return;

        if (enemies.IsBaseBeingAttacked)
            currentState.ChangeState(new EnemyDefendingBaseState(this, agent));
        else
            currentState.ChangeState(new EnemyMovingForwardState(this, agent));
    }

    void OnDisable() {
        // Anular la suscripción al evento cuando el objeto se desactiva
        PlayerManager.OnPlayerLookAtEnemy -= ShowHideCanvas;

        ResetEnemy();
    }

    void Start() {
        health = maxHealth;
        UpdateUI();
        playerBase = enemies.AttackersBase;

        enemyCanvasGO.SetActive(false);


        currentState = new EnemyMovingForwardState(this, agent);

        isStarted = true;
    }

    void Update() {
        if (!LevelManager.IsLevelOnGoing) {
            if (agent.remainingDistance > 0f) {
                agent.SetDestination(transform.position);
                agent.ResetPath();
            }
            return;
        }

        currentState = currentState.Process();
        meStateNow = currentState.currentState;

        if (!isDead) {
            if (attackTarget != null) {
                if (!attackTarget.gameObject.activeInHierarchy) {
                    attackTarget = null;
                }
            }

            if (attackTarget == null) {
                CheckNearby();
            }
        }

        // Si el jugador deja de mirar al enemigo y el canvas está activo
        if (!isPlayerLookingAtEnemy && enemyCanvasGO.activeInHierarchy && !isHideCanvasCoActive) {
            // Comenzar la cuenta atrás para ocultar el canvas
            hideCanvasAfterTimeCo = StartCoroutine(HideCanvasAfterTimeCo(canvasDisplayTime));
        }
    }

    void CheckNearby() {
        // Detect the firt ally NPC or Player that is on attack range using OverlapSphere.
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, targetMasks);
        attackTarget = null; // Reset the attack target

        foreach (Collider collider in colliders) {
            if (collider.gameObject != gameObject && collider.gameObject.activeInHierarchy) {
                // Check if there is an obstacle between the enemy and the target
                if (Physics.Raycast(transform.position, collider.gameObject.transform.position - transform.position, attackRange, obstacleMask)) {
                    continue; // Skip this iteration if there is an obstacle between the enemy and the target
                }
                else {
                    attackTarget = collider.gameObject.transform;
                    return; // Stop the loop once the first valid target is found
                }
            }
        }
    }

    void ShowHideCanvas(Transform enemyTransform) {
        // Firts attempt to show the canvas
        /*// Si el jugador está mirando al enemigo y la vida del enemigo no está completa
        if (Camera.main.ScreenToWorldPoint(new Vector3(centerWidth, centerHeight, 0)) == transform.position && health < maxHealth) {
            // Activar el canvas de salud
            enemyCanvasGO.SetActive(true);
        }
        else if (enemyCanvasGO.activeInHierarchy) {
            // Desactivar el canvas de salud
            enemyCanvasGO.SetActive(false);
        }*/

        // Si el jugador está mirando a este enemigo y la vida del enemigo no está completa
        if (enemyTransform == transform && health < maxHealth) {
            // Activar el canvas de salud
            enemyCanvasGO.SetActive(true);
            // Indicar que el jugador está mirando al enemigo
            isPlayerLookingAtEnemy = true;
        }
        /*else {  // No llega a llamarse.
            // Indicar que el jugador no está mirando al enemigo
            isPlayerLookingAtEnemy = false;
        }*/

        isPlayerLookingAtEnemy = false;
    }

    IEnumerator HideCanvasAfterTimeCo(float time) {
        isHideCanvasCoActive = true;

        // Esperar el tiempo especificado
        yield return new WaitForSeconds(time);

        // Desactivar el canvas de salud
        enemyCanvasGO.SetActive(false);
        isHideCanvasCoActive = false;
    }

    void ResetEnemy() {
        if (hideCanvasAfterTimeCo != null) {
            StopCoroutine(hideCanvasAfterTimeCo);
            isHideCanvasCoActive = false;
        }

        if (enemyCanvasGO.activeInHierarchy) {
            enemyCanvasGO.SetActive(false);
        }

        attackTarget = null;
        health = maxHealth;
        isDead = false;

        // Error when exiting play mode

        try {
            if (gameObject != null)
                enemies?.MoveEnemyToRandomSpawn(transform);
        }
        catch (MissingReferenceException) {
            // Skip
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnBecameInvisible() {
        if (!Application.isPlaying)
            return;

        if (enemyCanvasGO.activeInHierarchy) {
            enemyCanvasGO.SetActive(false);

            if (hideCanvasAfterTimeCo != null) {
                StopCoroutine(hideCanvasAfterTimeCo);
                isHideCanvasCoActive = false;
            }
        }
    }
}