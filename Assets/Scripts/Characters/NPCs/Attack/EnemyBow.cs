using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBow : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private EnemyArcher me { get; set; }

    [field: Header("Shoot settings")]
    [field: SerializeField] private Rigidbody arrowPrefab { get; set; }
    [field: SerializeField] private Transform arrowSpawnPoint { get; set; }
    [field: SerializeField] private float maxShootForce { get; set; } = 100f;
    [field: SerializeField, Range(0, 2)] private float precision { get; set; } = 1f;

    private Rigidbody currentArrow { get; set; }
    private bool isDrawing { get; set; }
    private float drawStartTime { get; set; }

    private bool inCooldown { get; set; }

    /*void Update() {
        if (me.AttackTarget != null && !inCooldown && !isDrawing) {
            StartCoroutine(AttackCo());
        }
    }*/

    void StartDrawingBow() {
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(me.AttackTarget.position - transform.position));
        if (gameObject.CompareTag("Enemy")) {
            currentArrow.gameObject.layer = LayerMask.NameToLayer("ArrowEnemy");
        }
        currentArrow.transform.parent = arrowSpawnPoint;
        drawStartTime = Time.time;
        isDrawing = true;
    }

    void ReleaseArrow() {
        float drawDuration = Time.time - drawStartTime;
        float distanceToTarget = Vector3.Distance(transform.position, me.AttackTarget.position);
        float shootForce = Mathf.Min(distanceToTarget * precision, maxShootForce);
        //float actualShootForce = Mathf.Clamp(shootForce * drawDuration, 0, shootForce);
        float actualShootForce = shootForce;    /// TODO: Remove this line someday
        currentArrow.transform.parent = null;
        Rigidbody arrowRigidbody = currentArrow;
        arrowRigidbody.isKinematic = false;
        arrowRigidbody.AddForce((me.AttackTarget.position - transform.position).normalized * actualShootForce, ForceMode.Impulse);
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

        ReleaseArrow();

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