using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class BeamManipulationController {
    
    private ObjectSelector objectSelector;

    private Player player;
    private TransportableObject transportable;
    private CinemachineVirtualCamera vcam;

    private bool activated;
    private bool choosing;

    public bool IsActivated => activated;

    public BeamManipulationController(ObjectSelector objectSelector) {
        this.objectSelector = objectSelector;
    }

    public void OnActivated(Player player, CinemachineVirtualCamera vcam) {
        this.activated = true;
        this.transportable = player.GrabbedObject;
        this.player = player;
        this.vcam = vcam;
        vcam.m_Priority += 2;
    }

    public void OnUpdate() {
        HandleInput();
    }

    public void OnDeactivated() {
        activated = false;
        vcam.m_Priority -= 2;
    }

    private void HandleInput() {
        if (activated) {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                HandleClick();
            }
        }
    }

    private void HandleClick() {
        if (!choosing) {
            if (objectSelector.Selected != null && objectSelector.Selected.transform.parent.gameObject == transportable.gameObject) {
                transportable.GetComponentInChildren<SelectableObject>().Highlight();
                transportable.DestinationTrigger.gameObject.SetActive(true);
                choosing = true;
            }
        } else {
            if (objectSelector.Selected != null && objectSelector.Selected == transportable.DestinationTrigger) {
                transportable.GetComponentInChildren<SelectableObject>().StopHighlighting();
                transportable.DestinationTrigger.gameObject.SetActive(false);
                player.ActivateLayOut(transportable.DestinationTrigger.transform);
                choosing = false;
            }
        }
    }
}
