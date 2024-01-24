using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RespawnManager : MonoBehaviour {

	[field: Header("--- Respawn manager properties ---")]
	[field: SerializeField] public bool respawnEnabled { get; set; } = true;

    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private List<Respawn> enemyList { get; set; } = new List<Respawn>();
    [field: SerializeField, ReadOnlyField] private List<Respawn> allyList { get; set; } = new List<Respawn>();
    public int EnemyListCount { get => enemyList.Count; }
    public int AllyListCount { get => allyList.Count; }
    [field: SerializeField] private bool revalidateProperties { get; set; }

    void OnValidate() {
#if UNITY_EDITOR
        UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
        if (!isValidPrefabStage /*&& prefabConnected*/) {
            ValidateAssings();
        }
#endif
    }

    void ValidateAssings() {
        // Get all the enmies in the scene in a simplified way.
        if (enemyList.Count == 0 || revalidateProperties) {
            
            enemyList = FindObjectsByType<Respawn>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(x => x.ObjectType == Respawn.ObjectTypeEnum.Enemy).ToList();
        }
        if (allyList.Count == 0 || revalidateProperties) {
            allyList = FindObjectsByType<Respawn>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(x => x.ObjectType == Respawn.ObjectTypeEnum.Ally).ToList();
        }

        revalidateProperties = false;
    }


    public void AddToPool(Respawn respawn) {
        switch (respawn.ObjectType) {
            case Respawn.ObjectTypeEnum.Ally:
                allyList.Add(respawn);
                break;

            case Respawn.ObjectTypeEnum.Enemy:
                enemyList.Add(respawn);
                break;
        }
    }

    public void GetEnemyFromPool() {
        if (enemyList.Count > 0) {
            //EnemyRespawn enemy = (EnemyRespawn) enemyList[0];
            Respawn enemy = enemyList[0];
            //Debug.Log($"<color=yellow>Spawn Enemy: {enemy}</color>");
            enemyList.RemoveAt(0);
            enemy.gameObject.SetActive(true);
        }
    }

    public void GetAllyFromPool() {
        if (allyList.Count > 0) {
            //AllyRespawn ally = (AllyRespawn) allyList[0];
            Respawn ally = allyList[0];
            //Debug.Log($"<color=yellow>Spawn Ally: {ally}</color>");
            allyList.RemoveAt(0);
            ally.gameObject.SetActive(true);
        }
    }


}