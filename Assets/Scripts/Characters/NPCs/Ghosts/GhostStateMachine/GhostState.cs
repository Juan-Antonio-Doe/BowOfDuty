using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostState : NPCState {

    protected Ghost ghost { get; set; }
    protected GhostState nextState { get; set; }

    public GhostState(Ghost ghost, NavMeshAgent agent) {

        npc = ghost.gameObject;
        this.ghost = ghost;
        this.agent = agent;
        player = ghost.ghostsManager.Player;
        stage = STAGES.Enter;
    }

    public override void Enter() {
        //Debug.Log($"ArcherState: {npc.name} -> {currentState}");
        stage = STAGES.Update;

        //base.Enter();
    }

    public override void Update() {
        if (ghost.IsDead) {
            ChangeState(new GhostDeadState(ghost, agent));
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
    public GhostState Process() {
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
    public void ChangeState(GhostState nextState) {
        this.nextState = nextState;
        stage = STAGES.Exit;
    }

    protected bool PlayerDetected() {
        //return enemy.enemies.PlayerDetected;
        float distanceSquared = (npc.transform.position - player.transform.position).sqrMagnitude;
        if (distanceSquared <= ghost.AttackRange * ghost.AttackRange) {
            return true;
        }

        /*if (Vector3.Distance(npc.transform.position, player.transform.position) < enemy.AttackRange) {
            return true;
        }*/
        return false;
    }
}
