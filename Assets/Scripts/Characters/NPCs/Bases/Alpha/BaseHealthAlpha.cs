using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BaseHealthAlpha : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private LevelManager levelManager { get; set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private EnemiesManager enemiesManager { get; set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private AlliesManager alliesManager { get; set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private SphereCollider sphereTrigger { get; set; }

    [field: Header("Base properties")]
    [field: SerializeField] private float maxHealth { get; set; } = 100f;
    [field: SerializeField, ReadOnlyField] private float currentHealth { get; set; }

    [field: SerializeField] private bool isEnemyBase { get; set; }

    [field: SerializeField] private LayerMask attackersMask { get; set; }

    [field: Header("UI")]
    [field: SerializeField] private Image healthBar { get; set; }
    [field: SerializeField, ReadOnlyField] private Text healthText { get; set; }

    [field: SerializeField] private Text winnerText { get; set; }

    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private List<EnemyArcher> enemyArcherList { get; set; } = new List<EnemyArcher>();
    [field: SerializeField, ReadOnlyField] private List<AllyArcher> allyArcherList { get; set; } = new List<AllyArcher>();

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
            levelManager.EndLevel();
        } else {
            winnerText.text = $"{winner} <color=red>Enemy</color>";
            levelManager.EndLevel(false);
        }

        
    }

    void UpdateUI() {
        healthBar.fillAmount = currentHealth / maxHealth;
        healthText.text = $"{currentHealth} / {maxHealth}";
    }

    private void OnTriggerEnter(Collider other) {
        ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
        //OnTriggerEnterTest_1(other);
        OnTriggerEnterTest_2(other);
    }

    private void OnTriggerExit(Collider other) {
        ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);
        //OnTriggerExitTest_1(other);
        OnTriggerExitTest_2(other);
    }

    void OnTriggerEnterTest_1(Collider other) {
        if (isEnemyBase) {
            if (other.CompareTag("Ally")) {
                enemiesManager.IsBaseBeingAttacked = true;
                //Debug.Log("Enemy base is being attacked");
            }
        }
        else {
            if (other.CompareTag("Enemy")) {
                alliesManager.IsBaseBeingAttacked = true;
                //Debug.Log("Ally base is being attacked");
            }
        }
    }

    void OnTriggerExitTest_1(Collider other) {
        if (isEnemyBase) {
            if (other.CompareTag("Ally")) {
                if (Physics.OverlapSphere(transform.position, sphereTrigger.radius, attackersMask).Length == 0) {
                    enemiesManager.IsBaseBeingAttacked = false;
                    Debug.Log("Enemy base is not being attacked");
                }
            }
        }
        else {
            if (other.CompareTag("Enemy")) {
                if (Physics.OverlapSphere(transform.position, sphereTrigger.radius, attackersMask).Length == 0) {
                    alliesManager.IsBaseBeingAttacked = false;
                    Debug.Log("Ally base is not being attacked");
                }
            }
        }
    }

    void OnTriggerEnterTest_2(Collider other) {
        if (isEnemyBase) {
            if (other.CompareTag("Ally")) {
                AllyArcher allyArcher = other.GetComponent<AllyArcher>();
                if (!allyArcherList.Contains(allyArcher)) {
                    allyArcherList.Add(allyArcher);
                }
                enemiesManager.IsBaseBeingAttacked = true;
                //Debug.Log("Enemy base is being attacked");
            }
        }
        else {
            if (other.CompareTag("Enemy")) {
                EnemyArcher enemyArcher = other.GetComponent<EnemyArcher>();
                if (!enemyArcherList.Contains(enemyArcher)) {
                    enemyArcherList.Add(enemyArcher);
                }
                alliesManager.IsBaseBeingAttacked = true;
            }
        }
    }

    void OnTriggerExitTest_2(Collider other) {
        if (isEnemyBase) {
            if (other.CompareTag("Ally")) {
                AllyArcher allyArcher = other.GetComponent<AllyArcher>();
                if (allyArcherList.Contains(allyArcher)) {
                    allyArcherList.Remove(allyArcher);
                }
            }
            if (allyArcherList.Count == 0) {
                enemiesManager.IsBaseBeingAttacked = false;
                //Debug.Log("Enemy base is not being attacked");
            }
        }
        else {
            if (other.CompareTag("Enemy")) {
                EnemyArcher enemyArcher = other.GetComponent<EnemyArcher>();
                if (enemyArcherList.Contains(enemyArcher)) {
                    enemyArcherList.Remove(enemyArcher);
                }
            }
            if (enemyArcherList.Count == 0) {
                alliesManager.IsBaseBeingAttacked = false;
                //Debug.Log("Ally base is not being attacked");
            }
        }
    }
}
