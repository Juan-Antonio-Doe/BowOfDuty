using UnityEngine.AI;

internal class GhostDeadState : GhostState {
    public GhostDeadState(Ghost ghost, NavMeshAgent agent) : base(ghost, agent) {
        currentState = STATE.Dead;

    }

    public override void Enter() {
        agent.isStopped = true;
        agent.ResetPath();

        base.Enter();
    }
}