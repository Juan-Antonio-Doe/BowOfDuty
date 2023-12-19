using UnityEngine.AI;

internal class ArcherAttackingBaseState : ArcherState {
    public ArcherAttackingBaseState(EnemyArcher enemy, NavMeshAgent agent) : base(enemy, agent) {
        currentState = STATE.AttackingBase;
    }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (!TargetDetected()) {
            ChangeState(new ArcherMovingForwardState(enemy, agent));
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