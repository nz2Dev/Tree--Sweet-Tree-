using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorQuestZone : MonoBehaviour {

    [SerializeField] private DoorQuestElement initResident;

    private DoorQuestElement resident;

    public bool HasResident => resident != null;

    private void Start() {
        SetResident(initResident);
    }

    public void UpliftResident() {
        SetResident(null);
    }

    public void SetResident(DoorQuestElement resident) {
        if (this.resident != null) {
            this.resident.OnHostAssigned(null);
        }

        this.resident = resident;
        if (resident != null) {
            resident.transform.position = transform.position;
            resident.transform.rotation = transform.rotation;
        }
        if (resident != null) {
            resident.OnHostAssigned(this);
        }
    }
}
