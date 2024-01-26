using UnityEngine.AI;

internal class AllyDefendingBaseState : AllyArcherState {
    public AllyDefendingBaseState(AllyArcher ally, NavMeshAgent agent) : base(ally, agent) {
        currentState = STATE.DefendingBase;

        agent.speed = ally.MoveSpeed;
        agent.isStopped = false;
        agent.autoBraking = false;
    }

    public override void Update() {
        base.Update();

        CheckEnemyDetected();

        if (!ally.AlliesManager.IsBaseBeingAttacked) {
            ChangeState(new AllyMovingForwardState(ally, agent));
        }

        agent.SetDestination(ally.AlliesManager.EnemiesManager.AttackersBase.position);
    }

    public override void Exit() {
        agent.ResetPath();

        base.Exit();
    }
}