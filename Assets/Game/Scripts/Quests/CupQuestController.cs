using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestElementItem {
    public GameObject elementGO;
    public Vector3 initialPosition;
    public bool isInSpot;
}

public class CupQuestController : MonoBehaviour {

    [SerializeField] private ObjectSelector selector;
    [SerializeField] private CinemachineVirtualCamera questCamera;
    [SerializeField] private ActivationObject activationObject;
    [SerializeField] private TableStates tableStates;
    [SerializeField] private Transform elementsLocation;
    [SerializeField] private GameObject assemblyCenter;
    [SerializeField] private float elementsPlacementsOffset = 1.0f;
    [SerializeField] private Player player;
    [SerializeField] private GameObject assembledCupPickupablePrefab;
    [SerializeField] private float cameraCutDuration = 0.9f;

    private bool activated;
    private float activationStartTime;

    private Vector3 nextElementPlacementPosition;
    private List<QuestElementItem> questElementItems;

    private QuestElementItem activatedQuestItem;

    private void Awake() {
        activationObject.OnActivated += ActivationObjectOnActivated;
        questElementItems = new List<QuestElementItem>();
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
            initialPosition = placementPosition,
            isInSpot = false,
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

    private bool waitForPickUp;
    private float waitStartTime;
    private PickUpable pickUpable;

    private void OnFinish() {
        OnDeactivate();

        foreach (var item in questElementItems) {
            Destroy(item.elementGO);
        }
        questElementItems.Clear();

        pickUpable = Instantiate(assembledCupPickupablePrefab, assemblyCenter.transform.position, Quaternion.identity).GetComponent<PickUpable>();
        waitForPickUp = true;
        waitStartTime = Time.time;
    }

    private bool rotationStage;
    private Quaternion rotationProgres;

    private void Update() {
        if (waitForPickUp) {
            if (Time.time > waitStartTime + cameraCutDuration) {
                waitForPickUp = false;
                player.ActivatePickUp(pickUpable, handleAutomatically: true);
            }
        }
        
        if (activated) {
            if (Input.GetKeyDown(KeyCode.F)) {
                OnFinish();
                return;
            }

            if (Time.time > activationStartTime + cameraCutDuration /*camera cut transition time*/ ) {
                player.GetComponentInChildren<Animator>(true).gameObject.SetActive(false);
            }

            if (Input.GetMouseButtonDown(1)) {
                OnDeactivate();
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                if (activatedQuestItem != null) {
                    HandleManipulationResult();
                } else if (selector.Selected != null) {
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

            if (activatedQuestItem != null) {
                var mousePointer = Camera.main.ScreenPointToRay(Input.mousePosition);
                var raycastCenter = new Plane(Vector3.up, assemblyCenter.transform.position);
                if (raycastCenter.Raycast(mousePointer, out float enter)) {
                    var raycastPoint = mousePointer.GetPoint(enter);
                    var finalTranslationPoint = raycastPoint;
                    var isSnappedToAssemblyCenter = Vector3.Distance(raycastPoint, assemblyCenter.transform.position) < 0.3;
                    SetIsRotationStage(isSnappedToAssemblyCenter);
                    if (isSnappedToAssemblyCenter) {
                        finalTranslationPoint = assemblyCenter.transform.position;
                    } else {
                        SetIsRotationInSpot(false);
                    }
                    activatedQuestItem.elementGO.transform.position = finalTranslationPoint;
                }

                if (rotationStage) {
                    var rotationDelta = Quaternion.AngleAxis(Input.mouseScrollDelta.y, Vector3.up);
                    rotationProgres *= rotationDelta;
                    if (rotationProgres.eulerAngles.y > 50 && rotationProgres.eulerAngles.y < 70) {
                        activatedQuestItem.elementGO.transform.rotation = Quaternion.Euler(0, 60, 0);
                        SetIsRotationInSpot(true);
                    } else {
                        activatedQuestItem.elementGO.transform.rotation = rotationProgres;
                        SetIsRotationInSpot(false);
                    }
                }
            }
        }
    }

    private void ActivateManipulationState(QuestElementItem questElementItem) {
        activatedQuestItem = questElementItem;
        activatedQuestItem.elementGO.GetComponent<CupQuestElement>().SetIsManipulationVisuals();
        assemblyCenter.SetActive(true);
    }

    private void HandleManipulationResult() {
        if (activatedQuestItem.isInSpot) {
            activatedQuestItem.elementGO.GetComponent<CupQuestElement>().SetSealed();
        } else {
            activatedQuestItem.elementGO.GetComponent<CupQuestElement>().Reset();
            activatedQuestItem.elementGO.transform.position = activatedQuestItem.initialPosition;
        }

        activatedQuestItem = null;
        assemblyCenter.SetActive(false);

        var allInSpot = true;
        if (questElementItems.Count == 0) {
            allInSpot = false;
        }
        foreach (var questElementItem in questElementItems) {
            if (!questElementItem.isInSpot) {
                allInSpot = false;
                break;
            }
        }

        if (allInSpot) {
            OnFinish();
        }
    }

    private void SetIsRotationStage(bool rotationStage) {
        if (!this.rotationStage && rotationStage) {
            rotationProgres = activatedQuestItem.elementGO.transform.rotation;
        }
        this.rotationStage = rotationStage;
        assemblyCenter.SetActive(!rotationStage);
    }

    private void SetIsRotationInSpot(bool isInSpot) {
        activatedQuestItem.isInSpot = isInSpot;
        activatedQuestItem.elementGO.GetComponent<CupQuestElement>().SetIsInSpotVisuals(isInSpot);
    }

}
