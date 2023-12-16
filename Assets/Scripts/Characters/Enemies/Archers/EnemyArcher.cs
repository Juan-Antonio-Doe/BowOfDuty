using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyArcher : Enemy {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] protected NavMeshAgent agent { get; set; }

    [field: Header("Move settings")]
    [field: SerializeField] private float moveSpeed { get; set; } = 4f;
    public float MoveSpeed { get => moveSpeed; }
    [field: SerializeField] private Transform moveForwardPoint { get; set; }
    public Transform MoveForwardPoint { get => moveForwardPoint; }

    [field: Header("Attack settings")]
    [field: SerializeField] private GameObject arrowPrefab { get; set; }
    [field: SerializeField] private Transform arrowSpawnPoint { get; set; }
    [field: SerializeField] private LayerMask targetMasks { get; set; }

    private ArcherState currentState { get; set; }

    private Transform attackTarget { get; set; }
    public Transform AttackTarget { get => attackTarget; }

    private bool isDead { get; set; }
    public bool IsDead { get => isDead; }

    void Start() {
        currentState.ChangeState(new ArcherMovingForwardState(this, agent));
    }

    void Update() {
        if (!isDead) {
            if (attackTarget != null) {
                if (!attackTarget.gameObject.activeSelf) {
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
            if (collider.gameObject != gameObject && collider.gameObject.activeSelf) {
                attackTarget = collider.gameObject.transform;
                return; // Stop the loop once the first valid target is found
            }
        }
    }
}