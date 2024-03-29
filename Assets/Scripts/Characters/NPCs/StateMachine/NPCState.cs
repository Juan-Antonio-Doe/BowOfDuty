using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCState {

    public enum STATE {
        Idle,               // This is the paused state.
        MovingForward,      // NPC is moving forward to the player's base.
        AttackingNPC,       // NPC is attacking the other NPC.
        AttackingBase,      // NPC is attacking the other NPC's base.
        AttackingPlayer,    // Enemy is attacking the player.
        DefendingBase,      // NPC is defending its base.
        Wander,             // Ghost is wandering around.
        TakeCover,          // Ghost is taking cover.
        RunAway,            // Ghost is running away.
        Dead                // NPC is dead.
    }

    public enum STAGES {
        Enter,
        Update,
        Exit
    }

    public STATE currentState { get; set; }
    protected STAGES stage { get; set; }
    protected GameObject npc { get; set; }  // The current character gameobject.
    protected PlayerManager player { get; set; }
    protected NavMeshAgent agent { get; set; }
    protected bool isAlreadyDead { get; set; }


    public virtual void Enter() {
        //Debug.Log($"ArcherState: {npc.name} -> {currentState}");
        //Debug.Log($"NPC");
        stage = STAGES.Update;
    }

    public virtual void Update() {
        stage = STAGES.Update;
    }

    public virtual void Exit() {
        stage = STAGES.Exit;
    }

    protected void SmoothLookAt(Transform target) {
        Vector3 direction = (target.position - npc.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    protected void DeleteArrowsOnBody(int minChildrens) {
        if (npc.transform.childCount > minChildrens) {
            // Destroy gameobject from the last child to the third child (inclusive).
            for (int i = npc.transform.childCount - 1; i >= minChildrens; i--) {
                if (npc.transform.GetChild(i) != null) {
                    GameObject.Destroy(npc.transform.GetChild(i).gameObject);
                    //Debug.Log($"Destroying arrow {i}");
                }
            }
        }
    }
}
