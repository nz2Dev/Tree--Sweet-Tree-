using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public struct QuestElementItem {
    public GameObject elementGO;
    public Vector3 initialPosition;
}

public class CupQuestController : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera questCamera;
    [SerializeField] private ActivationObject activationObject;
    [SerializeField] private TableStates tableStates;
    [SerializeField] private Transform elementsLocation;
    [SerializeField] private GameObject assemblyCenter;
    [SerializeField] private float elementsPlacementsOffset = 1.0f;
    [SerializeField] private Player player;
    [SerializeField] private float cameraCutDuration = 0.9f;

    private bool activated;
    private float activationStartTime;

    private Vector3 nextElementPlacementPosition;
    private List<QuestElementItem> questElementItems;

    private ObjectSelector selector;
    private QuestElementItem activatedQuestItem;

    private void Awake() {
        activationObject.OnActivated += ActivationObjectOnActivated;
        questElementItems = new List<QuestElementItem>();
        selector = player.GetComponent<ObjectSelector>();
    }

    private void Start() {
        nextElementPlacementPosition = elementsLocation.position;
        assemblyCenter.SetActive(false);
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
        questElementItems.Add(new QuestElementItem {
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

    private bool rotationStage;

    private void Update() {
        if (activated) {
            if (Time.time > activationStartTime + cameraCutDuration /*camera cut transition time*/ ) {
                player.GetComponentInChildren<Animator>(true).gameObject.SetActive(false);
            }

            if (Input.GetMouseButtonDown(1)) {
                OnDeactivate();
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                if (selector.Selected != null) {
                    var questItemSelected = false;
                    var selectedQuestItem = default (QuestElementItem);
                    foreach (var questItem in questElementItems) {
                        if (questItem.elementGO == selector.Selected.gameObject) {
                            selectedQuestItem = questItem;
                            questItemSelected = true;
                        }
                    }

                    if (questItemSelected) {
                        ActivateManipulationState(selectedQuestItem);
                    }
                }
            }


            if (activatedQuestItem.elementGO != null) {
                var mousePointer = Camera.main.ScreenPointToRay(Input.mousePosition);
                var raycastCenter = new Plane(Vector3.up, assemblyCenter.transform.position);
                if (raycastCenter.Raycast(mousePointer, out float enter)) {
                    var raycastPoint = mousePointer.GetPoint(enter);
                    var finalTranslationPoint = raycastPoint;
                    var isSnappedToAssemblyCenter = Vector3.Distance(raycastPoint, assemblyCenter.transform.position) < 0.3;
                    SetIsRotationStage(isSnappedToAssemblyCenter);
                    if (isSnappedToAssemblyCenter) {
                        finalTranslationPoint = assemblyCenter.transform.position;
                    }
                    activatedQuestItem.elementGO.transform.position = finalTranslationPoint;
                }
            }

            if (Input.GetKeyDown(KeyCode.F)) {
                questElementItems[0].elementGO.transform.position += Camera.main.transform.right;
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                questElementItems[0].elementGO.transform.position = questElementItems[0].initialPosition;
            }
        }
    }

    private void ActivateManipulationState(QuestElementItem questElementItem) {
        activatedQuestItem = questElementItem;
        activatedQuestItem.elementGO.GetComponent<CupQuestElement>().SetIsManipulationVisuals();
        assemblyCenter.SetActive(true);
    }

    private void SetIsRotationStage(bool rotationStage) {
        this.rotationStage = rotationStage;
        assemblyCenter.SetActive(!rotationStage);
    }

}
