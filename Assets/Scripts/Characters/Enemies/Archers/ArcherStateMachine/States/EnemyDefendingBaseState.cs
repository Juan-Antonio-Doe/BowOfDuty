using UnityEngine;
using UnityEngine.AI;

internal class EnemyDefendingBaseState : EnemyArcherState {
    public EnemyDefendingBaseState(EnemyArcher enemy, NavMeshAgent agent) : base(enemy, agent) {
        currentState = STATE.DefendingBase;

        agent.speed = enemy.MoveSpeed;
        agent.isStopped = false;
        agent.autoBraking = false;
    }

    public override void Update() {
        base.Update();
        //Debug.Log($"EnemyDefendingBaseState: {enemy.EnemiesManager.IsBaseBeingAttacked}");

        CheckAllyDetected();

        if (!enemy.enemies.IsBaseBeingAttacked) {
            ChangeState(new EnemyMovingForwardState(enemy, agent));
            return;
        }

        agent.SetDestination(enemy.enemies.AlliesManager.AttackersBase.position);

    }

    public override void Exit() {
        agent.ResetPath();

        base.Exit();
    }
}