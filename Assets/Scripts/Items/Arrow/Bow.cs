using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour {

    [field: Header("Shoot settings")]
    [field: SerializeField] protected Arrow arrowPrefab { get; set; }
    [field: SerializeField] public Transform arrowSpawnPoint { get; private set; }
    [field: SerializeField] protected float maxShootForce { get; set; } = 100f;
    [field: SerializeField, Range(0, 4)] protected float precision { get; set; } = 1f;

    protected Arrow currentArrow { get; set; }
    protected bool isDrawing { get; set; }

    protected bool inCooldown { get; set; }

    // For optimization purposes:
    protected float previousShootForce = 0f;
    protected float actualShootForce = 0f;

    protected virtual void StartDrawingBow() {
        currentArrow.transform.parent = arrowSpawnPoint;
        isDrawing = true;
    }

    protected virtual void ReleaseArrow(Transform target) {

        // With square root:
        /*float distanceToTarget = Vector3.Distance(transform.position, me.AttackTarget.position);
        float shootForce = Mathf.Min(distanceToTarget * precision, maxShootForce);
        //float actualShootForce = Mathf.Clamp(shootForce * drawDuration, 0, shootForce);
        float actualShootForce = shootForce;    /// TODO: Remove this line someday*/

        // Without square root:
        Vector3 diff = target.position - arrowSpawnPoint.position;

        // Patch to avoid the arrow to go too down when the target a Tower
        if (target.CompareTag("Tower"))
            diff.y += 2f;

        float distanceToTargetSquared = diff.sqrMagnitude;
        float shootForce = Mathf.Min(distanceToTargetSquared * (precision * precision), maxShootForce * maxShootForce);

        // Reuse the previous shoot force if it's not too different from the current one.
        if (Mathf.Abs(shootForce - previousShootForce) > 0.01f) {
            actualShootForce = Mathf.Sqrt(shootForce);
            previousShootForce = shootForce;
        }

        currentArrow.transform.parent = null;
        Rigidbody arrowRigidbody = currentArrow.rb;
        arrowRigidbody.isKinematic = false;
        //arrowRigidbody.AddForce((me.AttackTarget.position - transform.position).normalized * actualShootForce, ForceMode.Impulse);
        diff.y += 0.5f; // To make the arrow go a little bit higher
        arrowRigidbody.AddForce(diff.normalized * actualShootForce, ForceMode.Impulse);
        currentArrow.PlayFlySound();
        arrowRigidbody.useGravity = true;
        isDrawing = false;
        Destroy(currentArrow.gameObject, 5f);
    }
}