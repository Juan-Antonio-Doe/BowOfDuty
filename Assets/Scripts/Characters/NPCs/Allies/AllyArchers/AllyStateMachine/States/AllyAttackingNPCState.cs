using UnityEngine;
using UnityEngine.AI;

internal class AllyAttackingNPCState : AllyArcherState {
    public AllyAttackingNPCState(AllyArcher ally, NavMeshAgent agent) : base(ally, agent) {
        currentState = STATE.AttackingNPC;
    }

    public override void Update() {
        base.Update();

        if (!TargetDetected() || !ally.AttackTarget.CompareTag("Enemy")) {
            ChangeState(new AllyMovingForwardState(ally, agent));
            return;
        }

        SmoothLookAt(ally.AttackTarget);

        if (ally.Bow.CanShoot()) {
            ally.Bow.Shoot();
        }

        //Debug.Log($"CanShoot: {ally.Bow.CanShoot()}");
    }

    public override void Exit() {
        ally.Bow.CancelAttack();

        base.Exit();
    }
}