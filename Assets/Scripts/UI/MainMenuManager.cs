using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private LevelManager levelManager { get; set; }

    [field: Header("Main Menu UI")]
    [field: SerializeField] private GameObject mainMenuUI { get; set; }
    [field: SerializeField] private Toggle soundToggle { get; set; }
    
    private bool isSoundEnabled { get; set; } = true;

    void Start() {
        // Check if sound is enabled or not using PlayerPrefs.
        if (PlayerPrefs.HasKey("SoundEnabled")) {
            isSoundEnabled = PlayerPrefs.GetFloat("SoundEnabled") == 1 ? true : false;
            soundToggle.isOn = isSoundEnabled;
        } else {
            PlayerPrefs.SetFloat("SoundEnabled", 1);
            soundToggle.isOn = true;
        }

        levelManager.hud.SetActive(false);
        PauseManager.SetOnPause(false);
        levelManager.EnableDisableCursor(false);
    }

    public void StartGame() {
        levelManager.StartLevel();
        mainMenuUI.SetActive(false);
        levelManager.hud.SetActive(true);
        levelManager.EnableDisableCursor(true);
    }

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SwitchAudio(bool enabled) {
        if (!enabled) {
            isSoundEnabled = false;
            soundToggle.isOn = false;
            levelManager.audioMixer.SetFloat("MasterVolume", -80);
        }
        else {
            isSoundEnabled = true;
            soundToggle.isOn = true;
            levelManager.audioMixer.SetFloat("MasterVolume", 0);
        }

        // Save the sound state using PlayerPrefs. Float value 0 is false, 1 is true.
        PlayerPrefs.SetFloat("SoundEnabled", isSoundEnabled ? 1 : 0);
    }
}