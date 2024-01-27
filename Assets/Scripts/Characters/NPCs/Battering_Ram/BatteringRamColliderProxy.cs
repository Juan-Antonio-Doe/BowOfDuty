using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteringRamColliderProxy : MonoBehaviour {

    [field: SerializeField] private BatteringRam batteringRam { get; set; }

    public void TakeDamage(float damage) {
        batteringRam.TakeDamage(damage);
    }
}