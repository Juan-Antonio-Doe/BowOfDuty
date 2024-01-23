using UnityEngine.AI;

internal class GhostDeadState : GhostState {
    public GhostDeadState(Ghost ghost, NavMeshAgent agent) : base(ghost, agent) {
        currentState = STATE.Dead;

    }

    public override void Enter() {
        isAlreadyDead = true;
        agent.isStopped = true;
        agent.ResetPath();

        base.Enter();
    }

    /*public override void Update() {
        base.Update();

        if (!ghost.IsDead) {
            ChangeState(new GhostWanderState(ghost, agent));
        }
    }*/

    public override void Exit() {
        isAlreadyDead = false;

        base.Exit();
    }
}