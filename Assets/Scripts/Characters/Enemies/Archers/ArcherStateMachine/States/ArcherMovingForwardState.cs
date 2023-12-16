using UnityEngine.AI;

internal class ArcherMovingForwardState : ArcherState {
    public ArcherMovingForwardState(EnemyArcher enemy, NavMeshAgent agent) : base(enemy, agent) {
        currentState = STATE.MovingForward;

        agent.speed = enemy.MoveSpeed;
        agent.isStopped = false;
        agent.autoBraking = false;
    }

    public override void Enter() {


        base.Enter();
    }

    public override void Update() {
        base.Update();

        CheckersOnUpdate();

        agent.SetDestination(enemy.MoveForwardPoint.position);
    }

    public override void Exit() { 
        agent.ResetPath();

        base.Exit();
    }

    void CheckersOnUpdate() {
        CheckPlayerDetected();
        CheckNPCDetected();
    } 

    void CheckPlayerDetected() {
        if (PlayerDetected()) {
            //ChangeState(new ArcherAttackingPlayerState(enemy, agent));
            return;
        }
    }

    void CheckNPCDetected() {
        if (NPCDetected()) {
            //ChangeState(new ArcherAttackingNPCState(enemy, agent));
            return;
        }
    }
}