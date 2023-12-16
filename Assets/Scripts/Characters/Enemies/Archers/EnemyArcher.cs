using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyArcher : Enemy {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] protected NavMeshAgent agent { get; set; }

    [field: Header("Attack settings")]
    [field: SerializeField] private GameObject arrowPrefab { get; set; }
    [field: SerializeField] private Transform arrowSpawnPoint { get; set; }
	
    void Start() {
        
    }

    void Update() {
        
    }
}