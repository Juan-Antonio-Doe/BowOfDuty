using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBow : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private AllyArcher me { get; set; }

    [field: Header("Shoot settings")]
    [field: SerializeField] private Rigidbody arrowPrefab { get; set; }
    [field: SerializeField] public Transform arrowSpawnPoint { get; private set; }
    [field: SerializeField] private float maxShootForce { get; set; } = 100f;
    [field: SerializeField, Range(0, 4)] private float precision { get; set; } = 1f;

    private Rigidbody currentArrow { get; set; }
    private bool isDrawing { get; set; }

    private bool inCooldown { get; set; }

    // For optimization purposes:
    float previousShootForce = 0f;
    float actualShootForce = 0f;

    /*void Update() {
        if (me.AttackTarget != null && !inCooldown && !isDrawing) {
            StartCoroutine(AttackCo());
        }
    }*/

    void StartDrawingBow() {
        currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(me.AttackTarget.position - transform.position));
        /*if (gameObject.CompareTag("Enemy")) {
            currentArrow.gameObject.layer = LayerMask.NameToLayer("ArrowEnemy");
        }*/
        currentArrow.transform.parent = arrowSpawnPoint;
        isDrawing = true;
    }

    void ReleaseArrow() {

        // With square root:
        /*float distanceToTarget = Vector3.Distance(transform.position, me.AttackTarget.position);
        float shootForce = Mathf.Min(distanceToTarget * precision, maxShootForce);
        //float actualShootForce = Mathf.Clamp(shootForce * drawDuration, 0, shootForce);
        float actualShootForce = shootForce;    /// TODO: Remove this line someday*/

        // Without square root:
        Vector3 diff = me.AttackTarget.position - transform.position;
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
        //arrowRigidbody.AddForce((me.AttackTarget.position - transform.position).normalized * actualShootForce, ForceMode.Impulse);
        diff.y += 0.5f; // To make the arrow go a little bit higher
        arrowRigidbody.AddForce(diff.normalized * actualShootForce, ForceMode.Impulse);
        arrowRigidbody.useGravity = true;
        isDrawing = false;
        Destroy(currentArrow.gameObject, 5f);
    }

    public bool CanShoot() {
        //Debug.Log($"CanShoot: Not InCooldown: {!inCooldown} | Not IsDrawing: {!isDrawing} | AttackTarget: {me.AttackTarget != null}");
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
