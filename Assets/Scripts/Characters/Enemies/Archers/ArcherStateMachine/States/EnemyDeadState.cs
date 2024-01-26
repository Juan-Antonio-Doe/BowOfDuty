using UnityEngine.AI;

internal class EnemyDeadState : EnemyArcherState {
    public EnemyDeadState(EnemyArcher enemy, NavMeshAgent agent) : base(enemy, agent) {
        currentState = STATE.Dead;
    }

    public override void Enter() {

        /*agent.isStopped = true;
        enemy.gameObject.SetActive(false);*/
        isAlreadyDead = true;
        agent.isStopped = true;
        agent.ResetPath();

        DeleteArrowsOnBody();

        base.Enter();
    }

    public override void Exit() {
        isAlreadyDead = false;

        base.Exit();
    }
}