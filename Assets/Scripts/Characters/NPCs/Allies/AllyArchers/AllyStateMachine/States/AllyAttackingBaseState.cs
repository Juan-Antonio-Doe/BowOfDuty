using UnityEngine.AI;

internal class AllyAttackingBaseState : AllyArcherState {
    public AllyAttackingBaseState(AllyArcher ally, NavMeshAgent agent) : base(ally, agent) {
        currentState = STATE.AttackingBase;
    }

    public override void Update() {
        base.Update();

        if (!TargetDetected()) {
            ChangeState(new AllyMovingForwardState(ally, agent));
            return;
        }

        SmoothLookAt(ally.AttackTarget);
        //enemy.transform.LookAt(enemy.AttackTarget);

        if (ally.Bow.CanShoot()) {
            ally.Bow.Shoot();
        }
    }

    public override void Exit() {
        ally.Bow.CancelAttack();

        base.Exit();
    }
}