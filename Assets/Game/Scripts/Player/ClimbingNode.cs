using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ClimbingConnector {
    public JumpPlatform hopPlatform;
    public MovePlatform movePlatform;
    public JumpPlatform dropPlatform;
}

public class ClimbingNode : MonoBehaviour {
    
    [SerializeField] private ClimbingConnector connectorV1;
    [SerializeField] private ClimbingConnector connectorV2;
    [SerializeField] private ClimbingConnector[] connectors;

    public bool TryGetValidConnector(out ClimbingConnector validConnector) {
        validConnector = default;
        
        if (IsConnctorValid(connectorV1)) {
            validConnector = connectorV1;
            return true;
        } else if (IsConnctorValid(connectorV2)) {
            validConnector = connectorV2;
            return true;
        }

        foreach (var connector in connectors) {
            if (IsConnctorValid(connector)) {
                validConnector = connector;
                return true;
            }
        }

        return false;
    }

    private static bool IsConnctorValid(ClimbingConnector connector) {
        return connector.hopPlatform != null && connector.hopPlatform.IsActive;
    }

}
