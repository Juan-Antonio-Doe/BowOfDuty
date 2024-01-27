using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BatteringRam : MonoBehaviour {

    public enum STATE { 
        Stopped,    // The battering ram is stopped waiting for conditions to move.
        Moving,     // The battering ram is moving to the enemy base.
        Breaking,   // The battering ram is breaking the enemy base's door.
        Cooldown,   // The battering ram is disabled after hit the enemy base's door three times or after losing all its health.
        Destroyed   // The battering ram is destroyed after breaking the enemy base's door.
    }

    [field: Header("Autoattach On Editor Properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private NavMeshAgent agent { get; set; }

    [field: Header("BatteringRam")]
    [field: SerializeField] private bool isEnemyRam { get; set; }
    [field: SerializeField, ReadOnlyField] private float health { get; set; } = 100f;
    [field: SerializeField] private float maxHealth { get; set; } = 100f;
    [field: SerializeField] private float timeBeforeStartSpawn { get; set; } = 30f;
    [field: SerializeField] private int hitsBeforeCooldown { get; set; } = 2;
    private int hitsCounter { get; set; }
    [field: SerializeField] private float timeBetweenHits { get; set; } = 10f;
    private float timeCounter { get; set; }
    [field: SerializeField] private float cooldownTime { get; set; } = 10f;

    [field: Header("Waypoints")]
    [field: SerializeField] private Transform[] waypoints { get; set; }
    [field: SerializeField, ReadOnlyField] private int currentWaypoint { get; set; } = -1;

    [field: Header("UI")]
    [field: SerializeField] private Image healthBar { get; set; }
    [field: SerializeField, ReadOnlyField] private Text healthText { get; set; }
    [field: SerializeField, ReadOnlyField] private GameObject healthGroupGO { get; set; }
    [field: SerializeField] private bool revalidateProperties { get; set; }

    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private STATE currenState { get; set; } = STATE.Stopped;
    [field: SerializeField, ReadOnlyField] private bool isDead { get; set; }

    private Coroutine cooldownCo { get; set; }
    private Vector3 spawnPosition;

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
        if (healthText == null || revalidateProperties) {
            healthText = healthBar.transform.GetChild(0).GetComponent<Text>();
        }

        if (healthGroupGO == null || revalidateProperties) {
            healthGroupGO = healthBar.transform.parent.gameObject;
        }

        revalidateProperties = false;
    }

    IEnumerator Start() {
        health = maxHealth;
        spawnPosition = transform.position;

        yield return new WaitForSeconds(timeBeforeStartSpawn);

        /*if (waypoints.Length > 0) {
            currenState = STATE.Moving;
            agent.SetDestination(waypoints[0].position);
            currentWaypoint = 0;
        }*/
        currenState = STATE.Moving;
    }

    void Update() {
        if (!LevelManager.IsLevelOnGoing) {
            if (agent.remainingDistance > 0f) {
                agent.SetDestination(transform.position);
                agent.ResetPath();
            }
            currenState = STATE.Destroyed;
            return;
        }

        MiniStateMachine();
    }

    void MiniStateMachine() {
        if (currenState == STATE.Stopped) {
            Stopped();
        } else if (currenState == STATE.Moving) {
            Moving();
        } else if (currenState == STATE.Breaking) {
            Breaking();
        } else if (currenState == STATE.Cooldown) {
            if (cooldownCo == null)
                cooldownCo = StartCoroutine(CooldownCo());
        } else if (currenState == STATE.Destroyed) {
            Destroyed();
        }
    }

    void Stopped() {
        agent.isStopped = true;
    }

    void Moving() {
        agent.isStopped = false;

        if (agent.remainingDistance <= agent.stoppingDistance) {
            if (currentWaypoint < waypoints.Length - 1) {
                currentWaypoint++;
                agent.SetDestination(waypoints[currentWaypoint].position);
            }
            else {
                currenState = STATE.Breaking;
                agent.ResetPath();
            }
        }
    }

    void Breaking() {
        // ToDo: Trigger animation for hitting the enemy base's door.
        // ToDo: Manage the hit counter.

        if (hitsCounter != hitsBeforeCooldown) {
            if (timeCounter <= 0) {
                timeCounter = timeBetweenHits;
                hitsCounter++;
                HitDoor();
            }
            else {
                timeCounter -= Time.deltaTime;
            }
        }

        if (hitsCounter >= hitsBeforeCooldown) {
            currenState = STATE.Cooldown;
        }
    }

    IEnumerator CooldownCo() {
        agent.isStopped = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        healthGroupGO.SetActive(false);
        transform.position = spawnPosition;

        // ToDo: Waiting x time to be able to respawn again.
        yield return new WaitForSeconds(cooldownTime);

        currentWaypoint = 0;
        health = maxHealth;
        isDead = false;
        hitsCounter = 0;
        timeCounter = 0;
        UpdateUI();
        healthGroupGO.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        agent.ResetPath();
        agent.isStopped = false;
        currenState = STATE.Moving;
        cooldownCo = null;
    }

    void Destroyed() {
        // ToDo: Disable the battering ram for the rest of the level.
        Destroy(gameObject);
    }

    public void TakeDamage(float damage) {
        if (health > 0) {
            health -= damage;
            if (health <= 0) {
                health = 0;
                Die();
            }
            UpdateUI();
        }
    }

    void Die() {
        isDead = true;
        currenState = STATE.Cooldown;
    }

    void UpdateUI() {
        healthBar.fillAmount = health / maxHealth;
        healthText.text = $"{health} / {maxHealth}";
    }

    void HitDoor() {
        Debug.Log("Hit door");
    }
}