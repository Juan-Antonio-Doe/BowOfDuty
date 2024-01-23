using UnityEngine;
using UnityEngine.AI;

internal class GhostWanderState : GhostState {

    private float agentDoubledHeight { get; set; }

    public GhostWanderState(Ghost ghost, NavMeshAgent agent) : base(ghost, agent) {
        currentState = STATE.Wander;

        agentDoubledHeight = agent.height * 2f;
    }

    public override void Enter() {
        agent.isStopped = false;
        NewRandomSeed();
        Wander();

        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (PlayerDetected()) {
            // Take Cover
            //ChangeState(new GhostTakeCoverState(ghost, agent));
            ChangeState(new GhostRunAwayState(ghost, agent));
        }

        if (agent.remainingDistance <= agent.stoppingDistance) {
            Wander();
        }

    }

    public override void Exit() {
        agent.ResetPath();

        base.Exit();
    }

    void NewRandomSeed() {
        Random.InitState((int)System.DateTime.Now.Ticks + Time.frameCount);
    }

    void Wander() {
        //Debug.Log("Wandering");

        /*int[] _distValues = new int[6] { -6, -5, -4, 4, 5, 6 };

        Vector3 _randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        int _distance = _distValues[Random.Range(0, _distValues.Length)];*/

        int _distance = Random.Range(-20, 21);
        Vector3 _randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        /* Comprueba que la posición proporcionada esté dentro del NavMesh. Le pasamos la posición calculada al azar más la posición del agente
         * para que sea con respecto a donde se encuentra. El resto de valores puede dejarse siempre así.
         */
        if (NavMesh.SamplePosition(npc.transform.position + (_randomDir * _distance), out NavMeshHit _hit, agentDoubledHeight, NavMesh.AllAreas)) {
            agent.SetDestination(_hit.position);
        }
        else {
            Wander();
        }
    }
}