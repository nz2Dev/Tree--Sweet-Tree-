using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TransportableObjectDestination))]
public class TransportableObjectDestinationEvents : MonoBehaviour {

    [SerializeField] private UnityEvent OnContainObject;
    [SerializeField] private UnityEvent OnDoesNotContainObject;
    
    private void Awake() {
        var destination = GetComponent<TransportableObjectDestination>();
        destination.OnContainObjectIsChanged += (contain) => {
            if (contain) {
                OnContainObject?.Invoke();
            } else {
                OnDoesNotContainObject?.Invoke();
            }
        };
    }
}
