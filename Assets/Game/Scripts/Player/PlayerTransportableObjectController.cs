using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTransportableObjectController {
    
    private ObjectSelector objectSelector;

    private Player player;
    private TransportableObject transportable;
    private CinemachineVirtualCamera vcam;

    private bool activated;
    private bool choosing;

    public bool IsActivated => activated;

    public PlayerTransportableObjectController(ObjectSelector objectSelector) {
        this.objectSelector = objectSelector;
    }

    public void OnActivated(Player player, CinemachineVirtualCamera vcam) {
        this.activated = true;
        this.transportable = player.GrabbedObject;
        this.player = player;
        this.vcam = vcam;
        this.grabbed = false;
        vcam.m_Priority += 2;
    }

    private bool grabbed;

    public void OnUpdate() {
        if (!grabbed) {
            if (player.GrabbedObject != null) {
                grabbed = true;
                StartChoosing();
            }
        }
        
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
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1)) {
                transportable.GetComponentInChildren<SelectableObject>().StopHighlighting();
                transportable.SetDestinationsActive(false);
                player.ActivateLayOut(transportable.CancelDestination.gameObject);
                choosing = false;
            }
        }
    }

    private void HandleClick() {
        if (choosing) {    
            if (objectSelector.Selected != null && transportable.IsDestinationTrigger(objectSelector.Selected)) {
                if (!transportable.IsDestinationTriggerInRange(objectSelector.Selected)) {
                    player.ActivateJump(null);
                    return;
                }

                transportable.GetComponentInChildren<SelectableObject>().StopHighlighting();
                transportable.SetDestinationsActive(false);
                player.ActivateLayOut(objectSelector.Selected.gameObject);
                choosing = false;
            }
        }
    }

    private void StartChoosing() {
        transportable.GetComponentInChildren<SelectableObject>().Highlight();
        transportable.SetDestinationsActive(true);
        choosing = true;
    }
}
