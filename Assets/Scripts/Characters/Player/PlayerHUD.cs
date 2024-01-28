using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private LevelManager levelManager { get; set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private PlayerManager player { get; set; }

    [field: Header("Player UI")]
    [field: SerializeField] private Image healthBar { get; set; }
    [field: SerializeField, ReadOnlyField] private Text healthText { get; set; }
    [field: SerializeField, ReadOnlyField] private GameObject[] lives { get; set; }
    [field: SerializeField, ReadOnlyField] private Text ghostToSuppressText { get; set; }
    [field: SerializeField, ReadOnlyField] private GameObject ghostCounterGO { get; set; }
    [field: SerializeField, ReadOnlyField] private Image staminaBar { get; set; }
    [field: SerializeField] private Color ghostBarColor { get; set; }
    [field: SerializeField, ReadOnlyField] private Color staminaBarColor { get; set; }
    [field: SerializeField] private bool revalidateProperties { get; set; }

    private int maxGhostToSuppress { get; set; }

    void OnValidate() {
#if UNITY_EDITOR
        UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
        if (!isValidPrefabStage && prefabConnected) {
            // Variables that will only be checked when they are in a scene
            if (!Application.isPlaying) {
                if (revalidateProperties)
                    ValidateAssings();
            }
        }
#endif
    }

    void ValidateAssings() {
        if (healthBar == null) {
            revalidateProperties = false;
            return;
        }

        Transform playerStatsGO = healthBar.transform.parent.parent;

        if (healthText == null || revalidateProperties) {
            healthText = healthBar.transform.GetChild(0).GetComponent<Text>();
        }

        if (lives == null || lives.Length != 3 || revalidateProperties) {
            lives = new GameObject[3];
            Transform livesGO = playerStatsGO.GetChild(1);
            lives[0] = livesGO.GetChild(0).gameObject;
            lives[1] = livesGO.GetChild(1).gameObject;
            lives[2] = livesGO.GetChild(2).gameObject;
        }

        if (ghostToSuppressText == null || revalidateProperties) {
            ghostCounterGO = playerStatsGO.GetChild(2).gameObject;
            ghostToSuppressText = ghostCounterGO.transform.GetChild(0).GetComponent<Text>();
        }

        if (staminaBar == null || revalidateProperties) {
            staminaBar = playerStatsGO.GetChild(0).GetChild(1).GetComponent<Image>();
            staminaBarColor = staminaBar.color;
        }

        revalidateProperties = false;
    }

    public void UpdateHealth(float health, float maxHealth) {
        healthBar.fillAmount = health / maxHealth;
        healthText.text = $"{health} / {maxHealth}";
    }

    public void UpdateLives(int livesCount) {
        switch (livesCount) {
            case 3:
                lives[0].SetActive(true);
                lives[1].SetActive(true);
                lives[2].SetActive(true);
                break;
            case 2:
                lives[0].SetActive(true);
                lives[1].SetActive(true);
                lives[2].SetActive(false);
                break;
            case 1:
                lives[0].SetActive(true);
                lives[1].SetActive(false);
                lives[2].SetActive(false);
                break;
            case 0:
                lives[0].SetActive(false);
                lives[1].SetActive(false);
                lives[2].SetActive(false);
                break;
        }
    }

    public void UpdateGhostCounter(int ghostCount) {
        if (player.IsGhost) {
            if (!ghostCounterGO.activeInHierarchy) {
                maxGhostToSuppress = ghostCount;
                ghostCounterGO.SetActive(true);
            }
        }
        ghostToSuppressText.text = $"{ghostCount}";

        /*if (ghostCount == 0) {
            ghostCounterGO.SetActive(false);
            ghostToSuppressText.text = $"{maxGhostToSuppress}";
            staminaBar.color = staminaBarColor;
            staminaBar.fillAmount = 1;
        }*/
    }

    public void UpdateGhostTime(float ghostTime, float maxGhostTime) {
        staminaBar.color = ghostBarColor;
        staminaBar.fillAmount = ghostTime / maxGhostTime;

        /*if (ghostTime <= 0) {
            ghostCounterGO.SetActive(false);
            ghostToSuppressText.text = $"{maxGhostToSuppress}";
            staminaBar.color = staminaBarColor;
            staminaBar.fillAmount = 1;
        }*/
    }

    public void UpdateStamina(float stamina, float maxStamina) {
        staminaBar.fillAmount = stamina / maxStamina;
    }

    public void UpdateUIOnDie(int maxGhost) {
        ghostToSuppressText.text = $"{maxGhost}";
        ghostCounterGO.SetActive(true);
        staminaBar.color = ghostBarColor;
        staminaBar.fillAmount = 1;
    }

    public void UpdateUIOnResurrect() {
        ghostCounterGO.SetActive(false);
        staminaBar.color = staminaBarColor;
        staminaBar.fillAmount = 1;
    }
}