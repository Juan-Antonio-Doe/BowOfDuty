using UnityEngine.AI;

internal class ArcherAttackingNPCState : ArcherState {
    public ArcherAttackingNPCState(EnemyArcher enemy, NavMeshAgent agent) : base(enemy, agent) {
        currentState = STATE.AttackingNPC;
    }

    public override void Enter() {

        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (!TargetDetected() || !enemy.AttackTarget.CompareTag("Ally")) {
            ChangeState(new ArcherMovingForwardState(enemy, agent));
            return;
        }

        SmoothLookAt(enemy.AttackTarget);
    }

    public override void Exit() {
        base.Exit();
    }
}