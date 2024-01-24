using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRespawn : Respawn {

    [field: Header("Auttoattach on Editor properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private EnemiesManager enemies { get; set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private EnemyArcher enemy { get; set; }

    /*public override void OnEnable() {
        //base.OnEnable();

        *//*enemy.ResetEnemy1();
        enemies.MoveEnemyToRandomSpawn(transform);
        enemy.ResetEnemy2();*//*
    }*/

    public void ResetEnemy() {
        //enemy.ResetEnemy1();
    }
}