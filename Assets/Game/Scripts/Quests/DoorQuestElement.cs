using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorQuestElement : MonoBehaviour {

    [SerializeField] private DoorQuestZone desiredZone;

    private DoorQuestZone host;

    public DoorQuestZone Host => host;

    public bool IsOnDesiredHost => host == desiredZone;

    public void SetTransported() {
        if (host != null) {
            host.SetResident(null);
        }
    }

    internal void OnHostAssigned(DoorQuestZone zone) {
        host = zone;
    }

}
