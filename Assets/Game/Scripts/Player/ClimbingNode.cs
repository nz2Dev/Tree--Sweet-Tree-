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

    public bool TryGetValidConnector(out ClimbingConnector connector) {
        connector = default;
        if (IsConnctorValid(connectorV1)) {
            connector = connectorV1;
            return true;
        } else if (IsConnctorValid(connectorV2)) {
            connector = connectorV2;
            return true;
        }
        return false;
    }

    private static bool IsConnctorValid(ClimbingConnector connector) {
        return connector.hopPlatform != null && connector.hopPlatform.IsActive;
    }

}
