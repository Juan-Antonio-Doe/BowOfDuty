using Nrjwolf.Tools.AttachAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Respawn : MonoBehaviour {

	public enum ObjectTypeEnum {
        Undefined,
		Ammo,
		Weapon,
		Enemy,
        Ally,
        Ghost,
		VFX
    }

    [field: Header("Auttoattach on Editor properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] protected RespawnManager respawnManager { get; set; }

	[field: Header("--- Respawn properties ---")]
    [field: SerializeField] protected bool OnEnableEnabled { get; set; } = true;
    [field: SerializeField] protected bool OnDisableEnabled { get; set; } = true;
	[SerializeField, ReadOnlyField] protected Vector3 startPos = Vector3.zero;
	[SerializeField, ReadOnlyField] protected Quaternion startRot = Quaternion.identity;
    [field: SerializeField, ReadOnlyField] protected Transform startParent { get; set; }

	[field: SerializeField, ReadOnlyField] protected bool started { get; set; }

    [field: SerializeField] protected ObjectTypeEnum objectType { get; set; } = ObjectTypeEnum.Undefined;
    public ObjectTypeEnum ObjectType { get { return objectType; } }

    public virtual void OnEnable() {
        if (!started || !OnEnableEnabled)
			return;

        transform.SetParent(startParent);
        transform.localPosition = startPos;
        transform.localRotation = startRot;
    }

	public virtual void Start() {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        startParent = transform.parent;
        started = true;
    }

    public virtual void OnDisable() {
        if (!OnDisableEnabled)
            return;

        if (respawnManager.respawnEnabled) {
            respawnManager.AddToPool(this);
        }
    }
}