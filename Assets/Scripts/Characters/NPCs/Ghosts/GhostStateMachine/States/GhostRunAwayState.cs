using UnityEngine;
using UnityEngine.AI;

internal class GhostRunAwayState : GhostState {

    private float agentDoubledHeight { get; set; }

    private bool isRunningAway { get; set; }
    private int runAwayCount { get; set; }

    private float originalSpeed { get; set; }

    public GhostRunAwayState(Ghost ghost, NavMeshAgent agent) : base(ghost, agent) {
        currentState = STATE.RunAway;

        agentDoubledHeight = agent.height * 2f;
    }

    public override void Enter() {
        agent.isStopped = false;
        originalSpeed = agent.speed;
        agent.speed = agent.speed * 1.5f;
        CheckFront();

        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (agent.remainingDistance <= agent.stoppingDistance) {
            if (PlayerDetected() && !isRunningAway) {
                CheckFront();
            }
            else if (!isRunningAway) {
                ChangeState(new GhostWanderState(ghost, agent));
            }
        }
    }

    public override void Exit() {
        agent.ResetPath();
        agent.speed = originalSpeed;

        base.Exit();
    }

    void RunAway() {
        Vector3 _dirToPlayer = ghost.transform.position - ghost.ghostsManager.Player.transform.position;
        _dirToPlayer.y = 0; // Evitamos que no encuentre el punto de huida por estar en una posición más alta o más baja que el jugador

        int _rotation = Random.Range(-45, 46);

        _dirToPlayer = Quaternion.Euler(0, _rotation, 0) * _dirToPlayer.normalized;   // Rota el vector _dirToPlayer

        if (NavMesh.SamplePosition(ghost.transform.position + (_dirToPlayer * ghost.RunAwayDistance),
            out NavMeshHit _hit, agentDoubledHeight, NavMesh.AllAreas)) {
            agent.SetDestination(_hit.position);
            runAwayCount = 0;
        }
        else if (runAwayCount > 20) {
            return;
        }
        else {
            runAwayCount++;
            RunAway();
        }
    }

    void CheckFront() {
        if (Physics.SphereCast(ghost.CheckFrontOrigin.position, ghost.CheckFrontRadius, ghost.CheckFrontOrigin.forward,
            out RaycastHit _hit, ghost.CheckFrontDistance, ghost.CoverLayer)) {

            /*
             * Huir en una dirección contraria a dónde está el obstáculo
             * Para poder huir cuando encuentra algo que le bloquea el camino, tiene que saber
             * la dirección hacia ese obstaculo y buscar una dirección perpendicular aleatoria a esa.
             */
            Vector3 _dirToObstacle = _hit.transform.position - ghost.transform.position;
            _dirToObstacle.y = 0;

            // Para que elija una de las dos perpendiculares de forma aleatoria
            int _rotation = Random.Range(0, 2) == 0 ? 90 : -90;
            agent.SetDestination(Quaternion.Euler(0, _rotation, 0) * (_dirToObstacle.normalized * ghost.RunAwayDistance));

            /*int _random = Random.Range(0, 2);

            if (_random == 0) {
                // Huye en una dirección perpendicular a la del obstáculo
                agent.SetDestination(Quaternion.Euler(0, 90, 0) * (_dirToObstacle.normalized * npc.RunAwayDistance));
            }
            else {
                // Huye en la otra dirección perpendicular a la del obstáculo
                agent.SetDestination(Quaternion.Euler(0, -90, 0) * (_dirToObstacle.normalized * npc.RunAwayDistance));
                int _rotation = Random.Range(0, 2) == 0 ? 90 : -90;
                agent.SetDestination(Quaternion.Euler(0, _rotation, 0) * (_dirToObstacle.normalized * npc.RunAwayDistance));
            }*/
        }
        else {  // Cuando no tiene obstáculos delante, calcula la dirección de huida
            RunAway();
        }
    }
}