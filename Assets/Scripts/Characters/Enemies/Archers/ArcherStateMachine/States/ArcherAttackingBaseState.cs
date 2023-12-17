using UnityEngine.AI;

internal class ArcherAttackingBaseState : ArcherState {
    public ArcherAttackingBaseState(EnemyArcher enemy, NavMeshAgent agent) : base(enemy, agent) {
        currentState = STATE.AttackingBase;
    }

    public override void Enter() {
        base.Enter();
    }
}