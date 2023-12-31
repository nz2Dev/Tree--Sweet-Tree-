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
    [SerializeField] private TransportableObjectDestination[] destinations;
    [SerializeField] private int initDestinationIndex;
    [SerializeField] private UnityEvent OnGrabbedEvent;
    [SerializeField] private UnityEvent OnLayedOutEvent;

    private GameObject layOutPlace;

    public Transform Offsets => hostOffset;
    public CinemachineVirtualCamera OverviewCam => vcam;

    public event Action<GameObject> OnLayedOutAt;

    private void Start() {
        foreach (var destination in destinations) {
            destination.gameObject.SetActive(false);
        }
        if (destinations.Length > 0) {
            destinations[initDestinationIndex].SetContainObject(true);
        }
    }

    public bool IsDestinationTrigger(SelectableObject selectable) {
        foreach (var destination in destinations) {
            // todo add selectable object reference in destination component
            if (destination.GetComponent<SelectableObject>() == selectable) {
                return true;
            }
        }
        return false;
    }

    public void SetDestinationsActive(bool activeState) {
        foreach (var destination in destinations) {
            destination.gameObject.SetActive(activeState && !destination.IsExcludedFromActivation);
        }
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
