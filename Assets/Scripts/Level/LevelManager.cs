using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    // Static Properties
    public static bool IsLevelOnGoing { get; private set; }


    [field: Header("Autoattach On Editor Properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private PlayerManager playerManager { get; set; }

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
    }

    public void StartLevel() {
        IsLevelOnGoing = true;
    }

    public void EndLevel() {
        IsLevelOnGoing = false;
    }
}