using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class TransportableObject : MonoBehaviour {
    
    [SerializeField] private Transform hostOffset;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private SelectableObject destinationTrigger;
    [SerializeField] private UnityEvent OnGrabbedEvent;
    [SerializeField] private UnityEvent OnLayedOutEvent;

    public Transform Offsets => hostOffset;
    public CinemachineVirtualCamera OverviewCam => vcam;
    public SelectableObject DestinationTrigger => destinationTrigger;

    public void OnGrabbed() {
        OnGrabbedEvent?.Invoke();
    }

    public void OnLayedOut() {
        OnLayedOutEvent?.Invoke();
    }
}
