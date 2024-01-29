using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBow : Bow {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private AllyArcher me { get; set; }

    private Coroutine attackCoroutine { get; set; }

    protected override void StartDrawingBow() {
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(me.AttackTarget.position - transform.position));
        
        base.StartDrawingBow();
    }

    public bool CanShoot() {
        //Debug.Log($"CanShoot: Not InCooldown: {!inCooldown} | Not IsDrawing: {!isDrawing} | AttackTarget: {me.AttackTarget != null}");
        return !inCooldown && !isDrawing && me.AttackTarget != null;
    }

    public void Shoot() {
        attackCoroutine = StartCoroutine(AttackCo());
    }

    IEnumerator AttackCo() {
        inCooldown = true;
        StartDrawingBow();

        yield return new WaitForSeconds(1f);

        if (me.AttackTarget == null)
            CancelAttack();
        else
            ReleaseArrow(me.AttackTarget);

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
}
