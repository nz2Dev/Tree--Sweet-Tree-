using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorQuestZone : MonoBehaviour {

    [SerializeField] private DoorQuestElement initResident;
    [SerializeField] private DoorQuestZone[] siblings;

    private DoorQuestElement resident;

    public bool HasResident => resident != null;

    private void Start() {
        SetResident(initResident);
    }

    public void UpliftResident() {
        SetResident(null);
    }

    public bool IsSiblingTo(DoorQuestZone zone) {
        foreach (var sibling in siblings) {
            if (sibling == zone) {
                return true;
            }
        }
        return false;
    }

    public void SetResident(DoorQuestElement resident) {
        if (this.resident != null) {
            this.resident.OnHostAssigned(null);
            this.resident.transform.SetParent(null, true);
        }

        this.resident = resident;
        if (resident != null) {
            resident.transform.position = transform.position;
            resident.transform.rotation = transform.rotation;
            resident.transform.SetParent(transform, true);
        }
        if (resident != null) {
            resident.OnHostAssigned(this);
        }
    }
}
