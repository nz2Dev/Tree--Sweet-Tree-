using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabActivator : MonoBehaviour {
    
    private TransportableObject transportableObject;

    private void Awake() {
        transportableObject = GetComponent<TransportableObject>();
    }

    public void Trigger(Player player) {
        player.ActivateGrab(transportableObject);
    }

}
