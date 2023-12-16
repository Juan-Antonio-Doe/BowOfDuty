using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ArcherState {

    public enum STATE {
        Idle,               // This is the paused state.
        MovingForward,      // Enemy is moving forward to the player's base.
        AttackingNPC,       // Enemy is attacking the player's allies.
        AttackingBase,      // Enemy is attacking the player's base.
        AttackingPlayer,    // Enemy is attacking the player.
        Dead                // Enemy is dead.
    }

    public enum STAGES {
        Enter,
        Update,
        Exit
    }

    public STATE currentState { get; set; }
    protected STAGES stage { get; set; }
    protected GameObject npc { get; set; }  // The current enemy gameobject.
    protected EnemyArcher enemy { get; set; }
    protected PlayerManager player { get; set; }
    protected NavMeshAgent agent { get; set; }
    protected ArcherState nextState { get; set; }

    public ArcherState(EnemyArcher enemy, NavMeshAgent agent) {

        this.npc = enemy.gameObject;
        this.enemy = enemy;
        this.agent = agent;
        this.player = enemy.enemies.Player;
        stage = STAGES.Enter;
    }

    public virtual void Enter() {
        stage = STAGES.Update;
    }

    public virtual void Update() {
        if (enemy.IsDead) {
            ChangeState(new ArcherDeadState(enemy, agent));
            stage = STAGES.Exit;
            return;
        }

        stage = STAGES.Update;
    }

    public virtual void Exit() {
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

    public bool PlayerDetected() {
        //return enemy.enemies.PlayerDetected;
        if (Vector3.Distance(npc.transform.position, player.transform.position) < enemy.AttackRange) {
            return true;
        }
        return false;
    }

    public bool NPCDetected() {
        return enemy.AttackTarget != null;
    }
}