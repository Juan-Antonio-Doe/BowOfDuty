using UnityEngine.AI;

internal class EnemyAttackingBaseState : EnemyArcherState {
    public EnemyAttackingBaseState(EnemyArcher enemy, NavMeshAgent agent) : base(enemy, agent) {
        currentState = STATE.AttackingBase;
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (!TargetDetected()) {
            ChangeState(new EnemyMovingForwardState(enemy, agent));
            return;
        }

        SmoothLookAt(enemy.AttackTarget);
        //enemy.transform.LookAt(enemy.AttackTarget);

        if (enemy.Bow.CanShoot()) {
            enemy.Bow.Shoot();
        }
    }

    public override void Exit() {
        enemy.Bow.CancelAttack();

        base.Exit();
    }

}