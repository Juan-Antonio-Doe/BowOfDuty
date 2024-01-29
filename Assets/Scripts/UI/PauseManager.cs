using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour {

    [field: Header("AutoAttach on Editor properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private LevelManager levelManager { get; set; }

    [field: Header("General pause properties")]
    [field: SerializeField] private GameObject _pauseMenu { get; set; }
    [field: SerializeField] private Toggle soundToggle { get; set; }
    [field: SerializeField, ReadOnlyField] public static bool onPause { get; private set; } = false;


    void Start() {
        _pauseMenu.SetActive(false);
    }

    void Update() {
        if (!LevelManager.IsLevelOnGoing)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }

    public void Pause() {
        if (PlayerPrefs.HasKey("SoundEnabled"))
            soundToggle.isOn = PlayerPrefs.GetFloat("SoundEnabled") == 1 ? true : false;

        if (!onPause) {
            onPause = true;
            Time.timeScale = 0;
            EnableDisableThings(true);
        }
        else {
            onPause = false;
            EnableDisableThings(false);
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Used for scripts with UI elements like pause menu for tell to other scripts that the game is paused.
    /// </summary>
    public static void SetOnPause(bool value) {
        onPause = value;
        if (value)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void EnableDisableThings(bool value) {
        _pauseMenu.SetActive(value);
        levelManager.EnableDisableCursor(!value);
        levelManager.hud.SetActive(!value);
    }
}