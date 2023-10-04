using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshExtensions {
    
    public static float ExtGetRemainingDistanceAnyState(this NavMeshAgent agent) {
        if (agent.pathPending) {
            return float.PositiveInfinity;
        } else {
            return agent.remainingDistance;
        }
    }

    public static float ExtGetCurrentSpeed(this NavMeshAgent agent) {
        return agent.velocity.magnitude;
    }

    public static void ExtResetDestination(this NavMeshAgent agent) {
        agent.destination = agent.nextPosition;
    }

}
