using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBow : Bow {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private Tower me { get; set; }

    [field: Header("Shoot settings")]
    [field: SerializeField] private Arrow enemyArrowPrefab { get; set; }
    [field: SerializeField] private Arrow allyArrowPrefab { get; set; }

    private Coroutine attackCoroutine { get; set; }

    void Update() {
        if (!LevelManager.IsLevelOnGoing)
            return;

        if (currentArrow != null && me.AttackTarget != null)
            currentArrow.transform.rotation = Quaternion.LookRotation(me.AttackTarget.position - arrowSpawnPoint.position);
    }

    protected override void StartDrawingBow() {
        if (me.isEnemyTower)
            currentArrow = Instantiate(enemyArrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(me.AttackTarget.position - arrowSpawnPoint.position));
        else
            currentArrow = Instantiate(allyArrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(me.AttackTarget.position - arrowSpawnPoint.position));

        base.StartDrawingBow();
    }

    public bool CanShoot() {
        return !inCooldown && !isDrawing && me.AttackTarget != null;
    }

    public void Shoot() {
        attackCoroutine = StartCoroutine(AttackCo());
    }

    IEnumerator AttackCo() {
        inCooldown = true;
        StartDrawingBow();

        yield return new WaitForSeconds(1f);

        if (me.AttackTarget != null)
            ReleaseArrow(me.AttackTarget);
        else
            CancelAttack();

        yield return new WaitForSeconds(me.AttackCooldown);
        inCooldown = false;
    }

    public void CancelAttack() {
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        inCooldown = false;
        isDrawing = false;
        if (currentArrow != null) {
            Destroy(currentArrow.gameObject);
        }
    }

    private void OnDrawGizmosSelected() {
        if (me.AttackTarget != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(arrowSpawnPoint.position, me.AttackTarget.position);
        }
    }
}