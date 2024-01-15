using UnityEngine.AI;

internal class AllyDeadState : AllyArcherState {
    public AllyDeadState(AllyArcher ally, NavMeshAgent agent) : base(ally, agent) {
        currentState = STATE.Dead;
    }

    public override void Enter() {
        base.Enter();

        agent.isStopped = true;
        ally.gameObject.SetActive(false);
    }
}