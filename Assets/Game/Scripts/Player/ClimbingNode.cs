using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingNode : MonoBehaviour {
    
    [SerializeField] private ClimbingNodeConnector[] connectors;

    private Collider selectionCollider;

    private void Awake() {
        selectionCollider = GetComponent<Collider>();
        foreach (var connector in connectors) {
            connector.dropPlatform.OnPlayerOnTopChanged += NodeDropPlatformOnPlayerOnTopChanged;
        }
    }

    private void NodeDropPlatformOnPlayerOnTopChanged(bool isOnTop) {
        if (isOnTop) {
            selectionCollider.enabled = false;
        }
    }

    public bool TryGetValidConnector(out ClimbingNodeConnector validConnector) {
        validConnector = default;

        foreach (var connector in connectors) {
            if (IsConnctorValid(connector)) {
                validConnector = connector;
                return true;
            }
        }

        return false;
    }

    private static bool IsConnctorValid(ClimbingNodeConnector connector) {
        return connector.hopPlatform != null && connector.hopPlatform.IsActive;
    }

}
