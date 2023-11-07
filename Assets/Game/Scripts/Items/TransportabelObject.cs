using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransportableObject : MonoBehaviour {
    
    [SerializeField] private Transform hostOffset;
    [SerializeField] private UnityEvent OnGrabbedEvent;

    public Transform Offsets => hostOffset;

    public void OnGrabbed() {
        OnGrabbedEvent?.Invoke();
    }

}
