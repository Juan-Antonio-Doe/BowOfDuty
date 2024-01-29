using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllyArcherState : NPCState {

    protected AllyArcher ally { get; set; }
    protected AllyArcherState nextState { get; set; }

    public AllyArcherState(AllyArcher ally, NavMeshAgent agent) {

        npc = ally.gameObject;
        this.ally = ally;
        this.agent = agent;
        stage = STAGES.Enter;
    }

    public override void Enter() {
        //Debug.Log($"ArcherState: {npc.name} -> {currentState}");
        stage = STAGES.Update;

        //base.Enter();
    }

    public override void Update() {
        if (ally.IsDead && !isAlreadyDead) {
            ChangeState(new AllyDeadState(ally, agent));
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
    public AllyArcherState Process() {
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
    public void ChangeState(AllyArcherState nextState) {
        this.nextState = nextState;
        stage = STAGES.Exit;
    }

    protected bool PlayerDetected() {
        float distanceSquared = (npc.transform.position - player.transform.position).sqrMagnitude;
        if (distanceSquared <= ally.AttackRange * ally.AttackRange) {
            return true;
        }
        return false;
    }

    protected bool TargetDetected() {
        if (ally.AttackTarget != null && ally.AttackTarget.gameObject.activeInHierarchy) {
            float distanceSquared = (npc.transform.position - ally.AttackTarget.position).sqrMagnitude;
            if (distanceSquared <= ally.AttackRange * ally.AttackRange) {
                return true;
            }
        }

        ally.AttackTarget = null;
        return false;
    }

    protected void CheckEnemyDetected() {
        if (TargetDetected()) {
            if (ally.AttackTarget.CompareTag("Enemy") || ally.AttackTarget.CompareTag("Tower") || ally.AttackTarget.CompareTag("BatteringRam")) {
                ChangeState(new AllyAttackingNPCState(ally, agent));
                return;
            }
            else if (ally.AttackTarget.CompareTag("EnemyBase")) {
                ChangeState(new AllyAttackingBaseState(ally, agent));
                return;
            }
        }
    }
}