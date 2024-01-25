using UnityEngine.AI;

internal class AllyMovingForwardState : AllyArcherState {
    public AllyMovingForwardState(AllyArcher ally, NavMeshAgent agent) : base(ally, agent) {
        currentState = STATE.MovingForward;

        agent.speed = ally.MoveSpeed;
        agent.isStopped = false;
        agent.autoBraking = false;
    }

    public override void Update() {
        base.Update();

        CheckersOnUpdate();

        agent.SetDestination(ally.EnemyBase.position);
    }

    public override void Exit() {
        agent.ResetPath();

        base.Exit();
    }

    void CheckersOnUpdate() {
        CheckAllyDetected();
    }
}