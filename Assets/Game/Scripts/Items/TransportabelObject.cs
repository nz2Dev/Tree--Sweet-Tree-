using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class TransportableObject : MonoBehaviour {
    
    [SerializeField] private Transform hostOffset;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private SelectableObject[] destinationTriggerers;
    [SerializeField] private UnityEvent OnGrabbedEvent;
    [SerializeField] private UnityEvent OnLayedOutEvent;

    private GameObject layOutPlace;

    public Transform Offsets => hostOffset;
    public CinemachineVirtualCamera OverviewCam => vcam;
    public SelectableObject[] DestinationTriggers => destinationTriggerers;
    public bool IsDestinationTrigger(SelectableObject destination) => destinationTriggerers.Contains(destination);

    public void Grabed() {
        OnGrabbedEvent?.Invoke();
        if (layOutPlace != null) {
            layOutPlace.GetComponentInChildren<JumpPlatform>().SetActive(false);
        }
    }

    public void LayOut(GameObject destination) {
        OnLayedOutEvent?.Invoke();
        destination.GetComponentInChildren<JumpPlatform>().SetActive(true);
        layOutPlace = destination;
    }
}
