using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TransportableObjectDestination))]
public class BeamClimbingActivator : MonoBehaviour {

    private TransportableObjectDestination destination;

    private void Awake() {
        destination = GetComponent<TransportableObjectDestination>();
        destination.OnContainObjectIsChanged += DestinationOnContainObjectIsChanged;
    }

    private void DestinationOnContainObjectIsChanged(bool containObject) {
        foreach (var platform in GetComponentsInChildren<JumpPlatform>()) {
            platform.SetActive(containObject);
        }
    }

}
