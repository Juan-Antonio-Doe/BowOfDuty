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
    [field: SerializeField, ReadOnlyField] private int ghostsRefunned { get; set; }
    public int GhostsRefunned { get { return ghostsRefunned; } 
        set { 
            ghostsRefunned = value; 
            if (ghostsRefunned >= ghostsRefunnedMaxCountForResurrect) {
                playerManager.ResurrectPlayer();
                ghostsRefunned = 0;
            }
        } 
    }

    [field: SerializeField] private int ghostsRefunnedMaxCountForResurrect { get; set; } = 10;

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