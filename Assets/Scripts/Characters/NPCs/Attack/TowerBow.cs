using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBow : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private Tower me { get; set; }

    [field: Header("Shoot settings")]
    [field: SerializeField] private Rigidbody enemyArrowPrefab { get; set; }
    [field: SerializeField] private Rigidbody allyArrowPrefab { get; set; }
    [field: SerializeField] public Transform arrowSpawnPoint { get; set; }
    [field: SerializeField] private float maxShootForce { get; set; } = 100f;
    [field: SerializeField, Range(0, 4)] private float precision { get; set; } = 1f;

    private Rigidbody currentArrow { get; set; }
    private bool isDrawing { get; set; }

    private bool inCooldown { get; set; }

    // For optimization purposes:
    float previousShootForce = 0f;
    float actualShootForce = 0f;

    void Update() {
        if (!LevelManager.IsLevelOnGoing)
            return;

        if (currentArrow != null && me.AttackTarget != null)
            currentArrow.transform.rotation = Quaternion.LookRotation(me.AttackTarget.position - arrowSpawnPoint.position);
    }

    void StartDrawingBow() {
        isDrawing = true;

        if (me.isEnemyTower)
            currentArrow = Instantiate(enemyArrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(me.AttackTarget.position - transform.position));
        else
            currentArrow = Instantiate(allyArrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(me.AttackTarget.position - transform.position));

        currentArrow.transform.parent = arrowSpawnPoint;
    }

    void ReleaseArrow() {
        // Without square root:
        Vector3 diff = me.AttackTarget.position - arrowSpawnPoint.position;
        float distanceToTargetSquared = diff.sqrMagnitude;
        float shootForce = Mathf.Min(distanceToTargetSquared * (precision * precision), maxShootForce * maxShootForce);

        // Reuse the previous shoot force if it's not too different from the current one.
        if (Mathf.Abs(shootForce - previousShootForce) > 0.01f) {
            actualShootForce = Mathf.Sqrt(shootForce);
            previousShootForce = shootForce;
        }

        currentArrow.transform.parent = null;
        Rigidbody arrowRigidbody = currentArrow;
        arrowRigidbody.isKinematic = false;
        diff.y += 0.5f; // To make the arrow go a little bit higher
        arrowRigidbody.AddForce(diff.normalized * actualShootForce, ForceMode.Impulse);
        arrowRigidbody.useGravity = true;
        isDrawing = false;
        Destroy(currentArrow.gameObject, 5f);
    }

    public bool CanShoot() {
        return !inCooldown && !isDrawing && me.AttackTarget != null;
    }

    public void Shoot() {
        StartCoroutine(AttackCo());
    }

    IEnumerator AttackCo() {
        inCooldown = true;
        StartDrawingBow();

        yield return new WaitForSeconds(1f);

        if (me.AttackTarget != null)
            ReleaseArrow();
        else
            CancelAttack();

        yield return new WaitForSeconds(me.AttackCooldown);
        inCooldown = false;
    }

    public void CancelAttack() {
        StopAllCoroutines();
        inCooldown = false;
        isDrawing = false;
        if (currentArrow != null) {
            Destroy(currentArrow.gameObject);
        }
    }
}