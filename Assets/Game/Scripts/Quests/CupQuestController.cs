using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CupQuestController : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera questCamera;
    [SerializeField] private ActivationObject activationObject;
    [SerializeField] private Player player;
    [SerializeField] private float cameraCutDuration = 0.9f;

    private bool activated;
    private float activationStartTime;

    private void Awake() {
        activationObject.OnActivated += ActivationObjectOnActivated;
    }

    private void ActivationObjectOnActivated() {
        activationStartTime = Time.time;
        questCamera.m_Priority += 2;
        activated = true;
    }

    private void OnDeactivate() {
        questCamera.m_Priority -= 2;
        player.GetComponentInChildren<Animator>(true).gameObject.SetActive(true);
        activated = false;
    }

    private void Update() {
        if (activated) {
            if (Time.time > activationStartTime + cameraCutDuration /*camera cut transition time*/ ) {
                player.GetComponentInChildren<Animator>(true).gameObject.SetActive(false);
            }

            if (Input.GetMouseButtonDown(1)) {
                OnDeactivate();
            }
        }
    }
}
