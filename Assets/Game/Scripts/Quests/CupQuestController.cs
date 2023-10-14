using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public struct QuestElement {
    public GameObject elementGO;
    public Vector3 initialPosition;
}

public class CupQuestController : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera questCamera;
    [SerializeField] private ActivationObject activationObject;
    [SerializeField] private TableStates tableStates;
    [SerializeField] private Transform elementsLocation;
    [SerializeField] private float elementsPlacementsOffset = 1.0f;
    [SerializeField] private Player player;
    [SerializeField] private float cameraCutDuration = 0.9f;

    private bool activated;
    private float activationStartTime;

    private Vector3 nextElementPlacementPosition;
    private List<QuestElement> questElements;

    private void Awake() {
        activationObject.OnActivated += ActivationObjectOnActivated;
        questElements = new List<QuestElement>();
    }

    private void Start() {
        nextElementPlacementPosition = elementsLocation.position;
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
        var placementPosition = nextElementPlacementPosition;
        var elementGO = GameObject.Instantiate(item.prefab, placementPosition, Quaternion.identity);
        questElements.Add(new QuestElement {
            elementGO = elementGO,
            initialPosition = placementPosition
        });

        nextElementPlacementPosition = placementPosition + elementsLocation.forward * elementsPlacementsOffset;
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

            if (Input.GetKeyDown(KeyCode.F)) {
                questElements[0].elementGO.transform.position += Camera.main.transform.right;
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                questElements[0].elementGO.transform.position = questElements[0].initialPosition;
            }
        }
    }
}
