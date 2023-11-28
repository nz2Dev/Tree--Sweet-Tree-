using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbingNode : MonoBehaviour {
    
    [SerializeField] private ClimbingNodeConnector[] connectors;
    [Space]
    [SerializeField] private ClimbingNode[] outNodes;

    private Collider selectionCollider;

    private void Awake() {
        selectionCollider = GetComponent<Collider>();
        foreach (var connector in connectors) {
            connector.dropPlatform.OnPlayerOnTopChanged += NodeDropPlatformOnPlayerOnTopChanged;
        }
        foreach (var outNode in outNodes) {
            foreach (var outConnector in outNode.connectors) {
                outConnector.hopPlatform.OnPlayerOnTopChanged += OutNodeHopPlatformOnPlayerOnTopChanged;
            }
        }
    }

    private void OutNodeHopPlatformOnPlayerOnTopChanged(bool isPlayerOnTop) {
        if (isPlayerOnTop) {
            selectionCollider.enabled = true;
        }
    }

    private void NodeDropPlatformOnPlayerOnTopChanged(bool isOnTop) {
        if (isOnTop) {
            selectionCollider.enabled = false;
        }
    }

    public bool TryGetClimbingConnector(Vector3 sourcePosition, out ClimbingNodeConnector climbingConnector) {
        climbingConnector = default;

        var validconnectors = GetValidConnectors();
        if (validconnectors.Count == 0) {
            return false;
        }

        int CompareConnectors(ClimbingNodeConnector connector1, ClimbingNodeConnector connector2) {
            var distance1 = Vector3.Distance(connector1.hopPlatform.jumpStartPoint.position, sourcePosition);
            var distance2 = Vector3.Distance(connector2.hopPlatform.jumpStartPoint.position, sourcePosition);
            return distance1 < distance2 ? -1 : 1;
        }

        validconnectors.Sort(CompareConnectors);
        climbingConnector = validconnectors[0];
        return true;
    }

    private List<ClimbingNodeConnector> GetValidConnectors() {
        var validConnectors = new List<ClimbingNodeConnector>();

        foreach (var connector in connectors) {
            if (IsConnctorValid(connector)) {
                validConnectors.Add(connector);
            }
        }

        return validConnectors;
    }

    private static bool IsConnctorValid(ClimbingNodeConnector connector) {
        return connector.hopPlatform != null && connector.hopPlatform.IsActive;
    }

}
