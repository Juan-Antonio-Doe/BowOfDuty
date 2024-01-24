using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyRespawn : Respawn {

    [field: Header("Auttoattach on Editor properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private AlliesManager allies { get; set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private AllyArcher ally { get; set; }

    public override void OnEnable() {
        //base.OnEnable();

        /*ally.ResetAlly1();
        allies.MoveEnemyToRandomSpawn(transform);
        ally.ResetAlly2();*/
    }
}
