using UnityEngine.AI;

internal class EnemyAttackingNPCState : EnemyArcherState {
    public EnemyAttackingNPCState(EnemyArcher enemy, NavMeshAgent agent) : base(enemy, agent) {
        currentState = STATE.AttackingNPC;
    }

    public override void Enter() {

        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (!TargetDetected() || !enemy.AttackTarget.CompareTag("Ally")) {
            ChangeState(new EnemyMovingForwardState(enemy, agent));
            return;
        }

        SmoothLookAt(enemy.AttackTarget);

        if (enemy.Bow.CanShoot()) {
            enemy.Bow.Shoot();
        }
    }

    public override void Exit() {
        enemy.Bow.CancelAttack();

        base.Exit();
    }
}