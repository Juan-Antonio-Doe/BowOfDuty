using UnityEngine;
using UnityEngine.AI;

internal class EnemyMovingForwardState : ArcherState {
    public EnemyMovingForwardState(EnemyArcher enemy, NavMeshAgent agent) : base(enemy, agent) {
        currentState = STATE.MovingForward;

        agent.speed = enemy.MoveSpeed;
        agent.isStopped = false;
        agent.autoBraking = false;
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();

        CheckersOnUpdate();

        agent.SetDestination(enemy.PlayerBase.position);
    }

    public override void Exit() { 
        agent.ResetPath();

        base.Exit();
    }

    void CheckersOnUpdate() {
        CheckAllyDetected();
    }
}