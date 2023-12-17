using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyArcher : Enemy {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] protected NavMeshAgent agent { get; set; }
    [field: SerializeField, ReadOnlyField] protected Transform playerBase { get; set; }
    public Transform PlayerBase { get => playerBase; }
    [field: SerializeField, GetComponent, ReadOnlyField] protected EnemyBow bow { get; set; }
    public EnemyBow Bow { get => bow; }

    [field: Header("Move settings")]
    [field: SerializeField] private float moveSpeed { get; set; } = 4f;
    public float MoveSpeed { get => moveSpeed; }

    [field: Header("Attack settings")]
    [field: SerializeField] private GameObject arrowPrefab { get; set; }
    [field: SerializeField] private Transform arrowSpawnPoint { get; set; }
    [field: SerializeField] private LayerMask targetMasks { get; set; }

    private ArcherState currentState { get; set; }

    private Transform attackTarget { get; set; }
    public Transform AttackTarget { get => attackTarget; set => attackTarget = value; }

    public bool IsDead { get => isDead; }

    void OnValidate() {
#if UNITY_EDITOR
        UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
        if (!isValidPrefabStage /*&& prefabConnected*/) {
            // Variables that will only be checked when they are in a scene
            if (!Application.isPlaying) {
                if (playerBase == null)
                    playerBase = GameObject.FindGameObjectWithTag("PlayerBase").transform;

                if (healthText == null)
                    healthText = healthBar.transform.GetChild(0).GetComponent<Text>();
            }
        }
#endif
    }

    void Start() {
        currentState = new ArcherMovingForwardState(this, agent);
    }

    void Update() {
        currentState = currentState.Process();

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

    protected override void Die() {
        //base.Die();
        isDead = true;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}