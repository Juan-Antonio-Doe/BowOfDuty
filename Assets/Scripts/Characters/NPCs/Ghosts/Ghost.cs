using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : NPC {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] protected NavMeshAgent agent { get; set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] public GhostsManager ghostsManager { get; private set; }

    [field: Header("Run Away")]
    [field: SerializeField] private float runAwayDistance = 7f;
    public float RunAwayDistance { get { return runAwayDistance; } }

    [field: SerializeField] private Transform checkFrontOrigin;
    public Transform CheckFrontOrigin { get { return checkFrontOrigin; } }
    [field: SerializeField] private float checkFrontRadius = 1f;
    public float CheckFrontRadius { get { return checkFrontRadius; } }
    [field: SerializeField] private float checkFrontDistance = 3.5f;
    public float CheckFrontDistance { get { return checkFrontDistance; } }

    [field: Header("Take Cover Properties")]
    [field: SerializeField] private float takeCoverDistance = 15f;
    public float TakeCoverDistance { get { return takeCoverDistance; } }

    [field: SerializeField] private LayerMask coverLayer;
    public LayerMask CoverLayer { get { return coverLayer; } }

    [field: SerializeField] private float coverFactor = -0.1f;
    public float CoverFactor { get { return coverFactor; } }

    [field: SerializeField] private float timeTakingCover = 5f;
    public float TimeTakingCover { get { return timeTakingCover; } }

    private GhostState currentState { get; set; }

    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private Transform playerTarget { get; set; }
    public Transform PlayerTarget { get => playerTarget; set => playerTarget = value; }

    void Start() {
        health = maxHealth;

        if (checkFrontOrigin == null)
            checkFrontOrigin = transform;

        currentState = new GhostWanderState(this, agent);
    }

    void Update() {
        if (!LevelManager.IsLevelOnGoing)
            return;

        currentState = currentState.Process();

        if (!isDead) {
            
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, takeCoverDistance);
    }
}
