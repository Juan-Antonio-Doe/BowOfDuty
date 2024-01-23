using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostsManager : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private PlayerManager player { get; set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] public LevelManager levelManager { get; private set; }
    public PlayerManager Player { get => player; }
}
