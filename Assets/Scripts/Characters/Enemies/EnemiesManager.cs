using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour {

	[field: Header("Autoattach properties")]
	[field: SerializeField, FindObjectOfType, ReadOnlyField] private PlayerManager player { get; set; }
	public PlayerManager Player { get => player; }

	private bool playerDetected { get; set; }
	public bool PlayerDetected { get => playerDetected; set => playerDetected = value; }
    
}