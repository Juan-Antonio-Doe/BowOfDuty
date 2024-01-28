using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

    // Static Properties
    public static bool IsLevelOnGoing { get; private set; }


    [field: Header("Autoattach On Editor Properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private PlayerManager playerManager { get; set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private RespawnManager respawnManager { get; set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private EnemiesManager enemiesManager { get; set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private AlliesManager alliesManager { get; set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private AudioSource audioSource { get; set; }

    [field: Header("Level UI Properties")]
    [field: SerializeField] public GameObject hud { get; set; }
    [field: SerializeField] private GameObject endLevelUI { get; set; }
    [field: SerializeField] private float endLevelTime { get; set; } = 5f;

    [field: Header("Level Audio Properties")]
    [field: SerializeField] public AudioMixer audioMixer { get; private set; }


    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private bool isLevelOnGoingDebug { get; set; }

    void Awake() {
        IsLevelOnGoing = false;
        isLevelOnGoingDebug = false;
    }

    void Start() {
        audioMixer = audioSource.outputAudioMixerGroup.audioMixer;

        //StartLevel();
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
        hud.SetActive(false);
        endLevelUI.SetActive(true);

        if (win) {
            endLevelUI.transform.GetChild(1).gameObject.SetActive(true);
        } else {
            endLevelUI.transform.GetChild(2).gameObject.SetActive(true);
        }

        StartCoroutine(BackToMainMenuCo());
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

    IEnumerator BackToMainMenuCo() {
        yield return new WaitForSeconds(endLevelTime);
        BackToMainMenu();
    }

    public void BackToMainMenu() {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void EnableDisableCursor(bool locked) {
        if (PauseManager.onPause)
            locked = false;

        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}