using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class AllyArcher : NPC {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] protected NavMeshAgent agent { get; set; }
    [field: SerializeField, ReadOnlyField] protected Transform enemyBase { get; set; }
    public Transform EnemyBase { get => enemyBase; }
    [field: SerializeField, GetComponent, ReadOnlyField] protected AllyBow bow { get; set; }
    public AllyBow Bow { get => bow; }
    [field: SerializeField] private bool revalidateProperties { get; set; } = false;

    [field: Header("Move settings")]
    [field: SerializeField] private float moveSpeed { get; set; } = 4f;
    public float MoveSpeed { get => moveSpeed; }

    [field: Header("Attack settings")]
    [field: SerializeField] private LayerMask targetMasks { get; set; }

    private AllyArcherState currentState { get; set; }

    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private Transform attackTarget { get; set; }
    public Transform AttackTarget { get => attackTarget; set => attackTarget = value; }
    [field: SerializeField, ReadOnlyField] private AllyArcherState.STATE meStateNow { get; set; }

    void OnValidate() {
#if UNITY_EDITOR
        UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
        if (!isValidPrefabStage/* && prefabConnected*/) {
            // Variables that will only be checked when they are in a scene
            if (!Application.isPlaying) {
                if (revalidateProperties)
                    ValidateAssings();
            }
        }
#endif
    }

    void ValidateAssings() {
        if (enemyBase == null || revalidateProperties) {
            enemyBase = GameObject.FindGameObjectWithTag("EnemyBase").transform.GetChild(0);
        }
        revalidateProperties = false;
    }

    void Start() {
        health = maxHealth;

        currentState = new AllyMovingForwardState(this, agent);
    }

    void Update() {
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
    }

    public void CheckNearby() {
        // Detect the firt ally NPC or Player that is on attack range using OverlapSphere.
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, targetMasks);
        attackTarget = null; // Reset the attack target

        foreach (Collider collider in colliders) {
            if (collider.gameObject != gameObject && collider.gameObject.activeInHierarchy) {
                attackTarget = collider.gameObject.transform;
                return; // Stop the loop once the first valid target is found
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
