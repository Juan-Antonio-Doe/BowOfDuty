using UnityEngine.AI;

internal class ArcherDeadState : ArcherState {
    public ArcherDeadState(EnemyArcher enemy, NavMeshAgent agent) : base(enemy, agent) {
        currentState = STATE.Dead;

        agent.isStopped = true;
    }

    public override void Enter() {

        base.Enter();
    }
}