using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class BeamManipulationController : MonoBehaviour {
    
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private ObjectSelector objectSelector;
    [SerializeField] private SelectableObject destinationTrigger;

    private bool activated;
    private bool choosing;

    private void Awake() {
        vcam.m_Priority = 9;
    }

    public void OnActivated() {
        activated = true;
        vcam.m_Priority += 2;
    }

    private void Update() {
        HandleInput();
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
            if (objectSelector.Selected != null && objectSelector.Selected.gameObject == Player.LatestInstance.GrabbedObject.gameObject) {
                Player.LatestInstance.GrabbedObject.GetComponent<SelectableObject>().Highlight();
                destinationTrigger.gameObject.SetActive(true);
                choosing = true;
            }
        } else {
            if (objectSelector.Selected != null && objectSelector.Selected == destinationTrigger) {
                Player.LatestInstance.GrabbedObject.GetComponent<SelectableObject>().StopHighlighting();
                Player.LatestInstance.ActivateLayOut(destinationTrigger.transform);
                destinationTrigger.gameObject.SetActive(false);
                choosing = false;
            }
        }
    }
}
