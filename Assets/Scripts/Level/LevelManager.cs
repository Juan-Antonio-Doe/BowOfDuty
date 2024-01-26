using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    // Static Properties
    public static bool IsLevelOnGoing { get; private set; }


    [field: Header("Autoattach On Editor Properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private PlayerManager playerManager { get; set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private RespawnManager respawnManager { get; set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private EnemiesManager enemiesManager { get; set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private AlliesManager alliesManager { get; set; }

    [field: Header("Level Properties")]
    [field: Header("Player")]
    [field: SerializeField, ReadOnlyField] private int ghostsRekilled { get; set; }
    public int GhostsRekilled { get { return ghostsRekilled; } 
        set { 
            ghostsRekilled = value; 
            if (ghostsRekilled >= ghostsRekilledMaxCountForResurrect) {
                playerManager.ResurrectPlayer();
                ghostsRekilled = 0;
            }
        } 
    }

    [field: SerializeField] private int ghostsRekilledMaxCountForResurrect { get; set; } = 10;

    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private bool isLevelOnGoingDebug { get; set; }
	
    void Start() {
        StartLevel();
    }

    void Update() {
        isLevelOnGoingDebug = IsLevelOnGoing;

        RespawnEnemies();
        RespawnAllies();
    }

    public void StartLevel() {
        IsLevelOnGoing = true;
    }

    public void EndLevel(bool win=true) {
        IsLevelOnGoing = false;

        if (win) {
            Debug.Log("You win!");
        } else {
            Debug.Log("Game Over");
        }
    }

    void RespawnEnemies() {
        if (!IsLevelOnGoing || !respawnManager.respawnEnabled)
            return;

        if (respawnManager.EnemyListCount > 0) {
            respawnManager.GetEnemyFromPool();
        }
    }

    void RespawnAllies() {
        if (!IsLevelOnGoing || !respawnManager.respawnEnabled)
            return;

        if (respawnManager.AllyListCount > 0) {
            respawnManager.GetAllyFromPool();
        }
    }
}