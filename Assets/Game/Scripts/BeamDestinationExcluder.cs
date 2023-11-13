using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TransportableObjectDestination))]
public class BeamDestinationExcluder : MonoBehaviour {
    
    [SerializeField] private TransportableObjectDestination excludedDestination;

    private void Awake() {
        var destination = GetComponent<TransportableObjectDestination>();
        destination.OnContainObjectIsChanged += (containObject) => {
            excludedDestination.SetIsExcluded(containObject);
        };
    }
}
