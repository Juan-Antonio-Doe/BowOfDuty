using UnityEngine.AI;

internal class ArcherDeadState : ArcherState {
    public ArcherDeadState(EnemyArcher enemy, NavMeshAgent agent) : base(enemy, agent) {
        currentState = STATE.Dead;
    }

    public override void Enter() {
        base.Enter();

        agent.isStopped = true;
        enemy.gameObject.SetActive(false);
    }
}