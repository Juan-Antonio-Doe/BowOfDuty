using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ArcherState : NPCState {

    /*public enum STATE {
        Idle,               // This is the paused state.
        MovingForward,      // Enemy is moving forward to the player's base.
        AttackingNPC,       // Enemy is attacking the player's allies.
        AttackingBase,      // Enemy is attacking the player's base.
        AttackingPlayer,    // Enemy is attacking the player.
        Dead                // Enemy is dead.
    }*/

    protected EnemyArcher enemy { get; set; }
    protected ArcherState nextState { get; set; }

    public ArcherState(EnemyArcher enemy, NavMeshAgent agent) {

        npc = enemy.gameObject;
        this.enemy = enemy;
        this.agent = agent;
        player = enemy.enemies.Player;
        stage = STAGES.Enter;
    }

    public override void Enter() {
        //Debug.Log($"ArcherState: {npc.name} -> {currentState}");
        Debug.Log($"Enemy");
        stage = STAGES.Update;

        //base.Enter();
    }

    public override void Update() {
        if (enemy.IsDead) {
            ChangeState(new ArcherDeadState(enemy, agent));
            return;
        }
        stage = STAGES.Update;

        //base.Update();
    }

    public override void Exit() {
        //base.Exit();
        stage = STAGES.Exit;
    }

    /// <summary>
    /// This method is used to switch between the different methods that change the state.
    /// </summary>
    public ArcherState Process() {
        if (stage == STAGES.Enter) Enter();
        if (stage == STAGES.Update) Update();
        if (stage == STAGES.Exit) {
            Exit();
            return nextState; // It returns us the state that would touch next.
        }

        // This would return us to the same state we are in if none of the above conditions are met.
        return this;
    }

    /// <summary>
    /// Changes the state of the enemy.
    /// </summary>
    public void ChangeState(ArcherState nextState) {
        this.nextState = nextState;
        stage = STAGES.Exit;
    }

    protected bool PlayerDetected() {
        //return enemy.enemies.PlayerDetected;
        float distanceSquared = (npc.transform.position - player.transform.position).sqrMagnitude;
        if (distanceSquared <= enemy.AttackRange * enemy.AttackRange) {
            return true;
        }

        /*if (Vector3.Distance(npc.transform.position, player.transform.position) < enemy.AttackRange) {
            return true;
        }*/
        return false;
    }

    protected bool TargetDetected() {
        if (enemy.AttackTarget != null && enemy.AttackTarget.gameObject.activeInHierarchy) {
            float distanceSquared = (npc.transform.position - enemy.AttackTarget.position).sqrMagnitude;
            if (distanceSquared <= enemy.AttackRange * enemy.AttackRange) {
                return true;
            }
        }

        enemy.AttackTarget = null;
        return false;
    }

    protected void CheckAllyDetected() {
        if (TargetDetected()) {
            if (enemy.AttackTarget.CompareTag("Player")) {
                ChangeState(new ArcherAttackingPlayerState(enemy, agent));
                return;
            }
            else if (enemy.AttackTarget.CompareTag("Ally")) {
                ChangeState(new ArcherAttackingNPCState(enemy, agent));
                return;
            }
            else if (enemy.AttackTarget.CompareTag("PlayerBase")) {
                ChangeState(new ArcherAttackingBaseState(enemy, agent));
                return;
            }
        }
    }
}