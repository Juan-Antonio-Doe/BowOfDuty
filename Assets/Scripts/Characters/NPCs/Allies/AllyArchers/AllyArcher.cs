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

    private ArcherState currentState { get; set; }

    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private Transform attackTarget { get; set; }
    public Transform AttackTarget { get => attackTarget; set => attackTarget = value; }

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
        //currentState = new EnemyMovingForwardState(this, agent);
    }
}
