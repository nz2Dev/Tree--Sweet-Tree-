using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    public event Action<GameObject> OnLayedOutAt;

    private void Awake() {
        foreach (SelectableObject trigger in destinationTriggerers) {
            trigger.gameObject.SetActive(false);
        }
    }

    public bool IsDestinationTrigger(SelectableObject destination) {
        return destinationTriggerers.Contains(destination);
    }

    public void Grabed() {
        OnGrabbedEvent?.Invoke();
        if (layOutPlace != null && layOutPlace.TryGetComponent<TransportableObjectDestination>(out var dstCompnent)) {
            dstCompnent.SetContainObject(false);
        }
    }

    public void LayOut(GameObject destination) {
        OnLayedOutEvent?.Invoke();
        if (destination.TryGetComponent<TransportableObjectDestination>(out var dstCompnent)) {
            dstCompnent.SetContainObject(true);
        }
        layOutPlace = destination;
        OnLayedOutAt?.Invoke(destination);
    }
}
