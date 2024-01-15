using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    // Static Properties
    public static bool IsLevelOnGoing { get; private set; }


    //[field: Header("Autoattach On Editor Properties")]

    [field: Header("Level Properties")]

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