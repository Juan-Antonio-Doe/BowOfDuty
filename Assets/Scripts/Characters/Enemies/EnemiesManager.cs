using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemiesManager : MonoBehaviour {

	[field: Header("Autoattach properties")]
	[field: SerializeField, FindObjectOfType, ReadOnlyField] private PlayerManager player { get; set; }
	public PlayerManager Player { get => player; }
    [field: SerializeField, ReadOnlyField] private List<Transform> enemySpawnPoints { get; set; } = new List<Transform>();
    [field: SerializeField] private bool revalidateProperties { get; set; }

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
        if (enemySpawnPoints == null || enemySpawnPoints.Count == 0 || revalidateProperties) {
            enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemyRespawnPoint").Select(x => x.transform).ToList();
        }
        revalidateProperties = false;
    }


    public void MoveEnemyToRandomSpawn(Transform enemy) {
        int randomSpawnIndex = GenerateRandomNumber(0, enemySpawnPoints.Count);

        enemy.position = NavMesh.SamplePosition(enemySpawnPoints[randomSpawnIndex].position, out NavMeshHit hit,
            1f, NavMesh.AllAreas) ? hit.position : enemySpawnPoints[randomSpawnIndex].position;
    }

    int GenerateRandomNumber(int minInclusive, int maxExclusive) {
        int seed = System.DateTime.Now.Millisecond + Time.frameCount;
        Random.InitState(seed);
        return Random.Range(minInclusive, maxExclusive);
    }

}