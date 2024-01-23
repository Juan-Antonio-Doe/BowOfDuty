using UnityEngine;
using UnityEngine.AI;

internal class GhostTakeCoverState : GhostState {

    private float agentDoubledHeight { get; set; }

    private bool isTakingCover { get; set; }

    public GhostTakeCoverState(Ghost ghost, NavMeshAgent agent) : base(ghost, agent) {
        currentState = STATE.TakeCover;

        agentDoubledHeight = agent.height * 2f;
    }

    public override void Enter() {
        agent.isStopped = false;
        TakeCover();

        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (agent.remainingDistance <= agent.stoppingDistance) {
            if (PlayerDetected() && !isTakingCover) {
                TakeCover();
            }
        }
    }

    public override void Exit() {
        agent.ResetPath();

        base.Exit();
    }

    void TakeCover() {
        isTakingCover = true;

        Debug.Log("Taking Cover");

        Collider[] _covers = Physics.OverlapSphere(npc.transform.position, ghost.TakeCoverDistance, ghost.CoverLayer);

        for (int i = 0; i < _covers.Length; i++) {
            if (_covers[i].bounds.size.y < agent.height) {
                continue;
            }

            Vector3 _coverPos = _covers[i].transform.position;

            // Buscar primero que la posición este dentro del NavMesh
            if (NavMesh.SamplePosition(_coverPos, out NavMeshHit _hit, agentDoubledHeight, NavMesh.AllAreas)) {
                // Bucar el borde del NavMesh lo más cercano posible a la posición del objeto de cobertura
                if (NavMesh.FindClosestEdge(_hit.position, out NavMeshHit _edge, NavMesh.AllAreas)) {


                    Vector3 _direction = ghost.ghostsManager.Player.transform.position - _edge.position;
                    _direction.y = 0;

                    /*
                     * Calculamos el producto escalar entre la normal del borde con respecto a la dirección del jugador.
                     * Cuando ese valor es > 0, ambos vectores estan uno en frente del otro.
                     * Cuando ese valor es < 0, los vectores miran en direcciones opuestas.
                     */
                    float _dot = Vector3.Dot(_edge.normal, _direction.normalized);

                    if (_dot <= ghost.CoverFactor) {  // Si el producto escalar es <= que el factor, es una cobertura válida
                        agent.SetDestination(_edge.position);
                        break;
                    }
                    else { // Cuando el producto escalar > que el factor, hay que buscar otra cobertura

                        if (NavMesh.SamplePosition(_coverPos - (_direction.normalized * 2f), out _hit, agentDoubledHeight, NavMesh.AllAreas)) {
                            /*
                            * Para buscar la posición contraria a la que hemos encontrado en la primera busqueda de cobertura,
                            * desplazamos esa posición en la misma dirección en la que se encuentra ese punto respecto al jugador.
                            */
                            if (NavMesh.FindClosestEdge(_hit.position, out _edge, NavMesh.AllAreas)) {
                                // Hay que realizar la misma comprobación que con el primer punto encontrado
                                _dot = Vector3.Dot(_edge.normal, _direction.normalized);

                                // Si el producto escalar es <= que el factor, es una cobertura válida
                                if (_dot <= ghost.CoverFactor) {
                                    agent.SetDestination(_edge.position);
                                    break;
                                }
                            }
                        }
                    }

                }
            }
        }

        isTakingCover = false;
    }
}