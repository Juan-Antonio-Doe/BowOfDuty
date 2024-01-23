using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : NPC {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] protected NavMeshAgent agent { get; set; }

    //private GhostState currentState { get; set; }

    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private Transform playerTarget { get; set; }
    public Transform PlayerTarget { get => playerTarget; set => playerTarget = value; }

    void Start() {
        health = maxHealth;

        //currentState = new EnemyMovingForwardState(this, agent);
    }

    void Update() {
        if (!LevelManager.IsLevelOnGoing)
            return;

        //currentState = currentState.Process();

        if (!isDead) {
            
        }
    }
}
