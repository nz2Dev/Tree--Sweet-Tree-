using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CupQuestController : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera questCamera;
    [SerializeField] private ActivationObject activationObject;
    [SerializeField] private Player player;

    private bool activated;

    private void Awake() {
        activationObject.OnActivated += ActivationObjectOnActivated;
    }

    private void ActivationObjectOnActivated() {
        questCamera.m_Priority += 2;
        activated = true;
    }

    public void OnCameraEndTransition() {
        if (activated) {
            player.GetComponentInChildren<Animator>().gameObject.SetActive(false);
        }
    }

    private void OnDeactivate() {
        questCamera.m_Priority -= 2;
        player.GetComponentInChildren<Animator>(true).gameObject.SetActive(true);
        activated = false;
    }

    private void Update() {
        if (activated) {
            if (Input.GetMouseButtonDown(1)) {
                OnDeactivate();
            }
        }
    }
}
