using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BaseHealthAlpha : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] protected LevelManager levelManager { get; set; }

    [field: Header("Base properties")]
    [field: SerializeField] private float maxHealth { get; set; } = 100f;
    [field: SerializeField, ReadOnlyField] private float currentHealth { get; set; }

    [field: SerializeField] private bool isEnemyBase { get; set; }

    [field: SerializeField] protected Image healthBar { get; set; }
    [field: SerializeField, ReadOnlyField] protected Text healthText { get; set; }

    [field: SerializeField] protected Text winnerText { get; set; }

    void OnValidate() {
#if UNITY_EDITOR
        UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
        if (!isValidPrefabStage /*&& prefabConnected*/) {
            // Variables that will only be checked when they are in a scene
            if (!Application.isPlaying) {

                if (healthBar != null && healthText == null)
                    healthText = healthBar.transform.GetChild(0).GetComponent<Text>();
            }
        }
#endif
    }

    void Start() {
        currentHealth = maxHealth;
        UpdateUI();
        winnerText.transform.parent.gameObject.SetActive(false);
    }

    void Update() {
        
    }

    public void TakeDamage(float damage) {
        if (currentHealth > 0) {
            currentHealth -= damage;
            if (currentHealth <= 0) {
                EndGame();
            }
            UpdateUI();
        }
    }

    private void EndGame() {
        string winner = "Winner:";
        winnerText.transform.parent.gameObject.SetActive(true);

        if (isEnemyBase) {
            winnerText.text = $"{winner} <color=green>Player</color>";
        } else {
            winnerText.text = $"{winner} <color=red>Enemy</color>"; 
        }

        levelManager.EndLevel();
    }

    void UpdateUI() {
        healthBar.fillAmount = currentHealth / maxHealth;
        healthText.text = $"{currentHealth} / {maxHealth}";
    }
}
