using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransportableObject : MonoBehaviour {
    
    [SerializeField] private Transform hostOffset;
    [SerializeField] private UnityEvent OnGrabbedEvent;
    [SerializeField] private UnityEvent OnLayedOutEvent;

    public Transform Offsets => hostOffset;

    public void OnGrabbed() {
        OnGrabbedEvent?.Invoke();
    }

    public void OnLayedOut() {
        OnLayedOutEvent?.Invoke();
    }
}
