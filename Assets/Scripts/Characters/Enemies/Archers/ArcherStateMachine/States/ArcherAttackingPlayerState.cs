using UnityEngine;
using UnityEngine.AI;

internal class ArcherAttackingPlayerState : ArcherState {

    public ArcherAttackingPlayerState(EnemyArcher enemy, NavMeshAgent agent) : base(enemy, agent) {
        currentState = STATE.AttackingPlayer;

        agent.speed = enemy.MoveSpeed / 1.5f;
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