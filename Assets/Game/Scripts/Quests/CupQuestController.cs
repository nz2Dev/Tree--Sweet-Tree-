using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CupQuestController : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera questCamera;
    [SerializeField] private ActivationObject activationObject;
    [SerializeField] private TableStates tableStates;
    [SerializeField] private Transform elementsLocation;
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
        tableStates.PushState(TableStates.State.Stationar);
        activated = true;
        
        var playerInventory = player.GetComponent<Inventory>();
        playerInventory.OnItemActivated += PlayerInventoryOnItemActivated;
    }

    private void PlayerInventoryOnItemActivated(Item item) {
        var elementGO = GameObject.Instantiate(item.prefab, elementsLocation.position, Quaternion.identity);
    }

    private void OnDeactivate() {
        questCamera.m_Priority -= 2;
        player.GetComponentInChildren<Animator>(true).gameObject.SetActive(true);
        tableStates.PopState();
        activated = false;

        var playerInventory = player.GetComponent<Inventory>();
        playerInventory.OnItemActivated -= PlayerInventoryOnItemActivated;
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
