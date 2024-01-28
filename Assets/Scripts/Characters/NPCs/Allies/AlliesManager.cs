using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class AlliesManager : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private EnemiesManager enemiesManager { get; set; }
    public EnemiesManager EnemiesManager { get { return enemiesManager; } }
    [field: SerializeField, ReadOnlyField] private Transform attackersBase { get; set; }
    public Transform AttackersBase { get { return attackersBase; } }
    [field: SerializeField, ReadOnlyField] private List<Transform> allySpawnPoints { get; set; } = new List<Transform>();
    [field: SerializeField] private bool revalidateProperties { get; set; }

    [field: SerializeField, ReadOnlyField] private bool isBaseBeingAttacked { get; set; }
    public bool IsBaseBeingAttacked { get { return isBaseBeingAttacked; } set { isBaseBeingAttacked = value; } }

    void OnValidate() {
#if UNITY_EDITOR
        UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
        if (!isValidPrefabStage && prefabConnected) {
            if (revalidateProperties)
                ValidateAssings();
        }
#endif
    }

    void ValidateAssings() {
        if (attackersBase == null || revalidateProperties) {
            attackersBase = GameObject.FindGameObjectWithTag("EnemyBase").transform.GetChild(0);
        }
        if (allySpawnPoints == null || allySpawnPoints.Count == 0 || revalidateProperties) {
            allySpawnPoints = GameObject.FindGameObjectsWithTag("AllyRespawnPoint").Select(x => x.transform).ToList();
        }
        revalidateProperties = false;
    }


    public void MoveAllyToRandomSpawn(Transform ally) {
        if (ally == null)
            return;

        int randomSpawnIndex = GenerateRandomNumber(0, allySpawnPoints.Count);

        ally.position = NavMesh.SamplePosition(allySpawnPoints[randomSpawnIndex].position, out NavMeshHit hit,
            1f, NavMesh.AllAreas) ? hit.position : allySpawnPoints[randomSpawnIndex].position;
    }

    int GenerateRandomNumber(int minInclusive, int maxExclusive) {
        int seed = System.DateTime.Now.Millisecond + Time.frameCount;
        Random.InitState(seed);
        return Random.Range(minInclusive, maxExclusive);
    }
}
