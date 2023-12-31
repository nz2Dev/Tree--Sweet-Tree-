using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportableGrabber : MonoBehaviour {
    
    [SerializeField] private TransportableObject transportableObject;

    public void Trigger(Player player) {
        player.ActivateGrab(transportableObject);
    }

}
