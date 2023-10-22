using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorQuestElement : MonoBehaviour {

    private DoorQuestZone host;

    public DoorQuestZone Host => host;

    public void SetTransported() {
        if (host != null) {
            host.SetResident(null);
        }
    }

    internal void OnHostAssigned(DoorQuestZone zone) {
        host = zone;
    }

}
