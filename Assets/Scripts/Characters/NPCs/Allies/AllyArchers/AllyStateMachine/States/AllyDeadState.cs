using UnityEngine.AI;

internal class AllyDeadState : AllyArcherState {
    public AllyDeadState(AllyArcher ally, NavMeshAgent agent) : base(ally, agent) {
        currentState = STATE.Dead;
    }

    public override void Enter() {
        /*agent.isStopped = true;
        enemy.gameObject.SetActive(false);*/
        isAlreadyDead = true;
        agent.isStopped = true;
        agent.ResetPath();

        DeleteArrowsOnBody();

        base.Enter();
    }

    public override void Exit() {
        isAlreadyDead = false;

        base.Exit();
    }
}