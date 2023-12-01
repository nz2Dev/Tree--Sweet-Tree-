using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class CupQuestController : MonoBehaviour {

    [SerializeField] private ObjectSelector selector;
    [SerializeField] private CinemachineVirtualCamera questCamera;   
    [SerializeField] private CupAssembler cupAssembler; 
    [SerializeField] private Player player;
    [SerializeField] private float cameraCutDuration = 0.9f;
    [SerializeField] private float scrollSpeed = 4f;

    private bool activated;
    private float activationStartTime;
    private QuestElementItem activatedQuestItem;

    public event Action OnExit;

    public void OnActivated() {
        activationStartTime = Time.time;
        questCamera.m_Priority += 2;
        activated = true;
        
        var playerInventory = player.GetComponent<Inventory>();
        playerInventory.OnItemActivated += PlayerInventoryOnItemActivated;
    }

    private void PlayerInventoryOnItemActivated(int itemIndex) {
        var item = player.GetComponent<Inventory>().GetItem(itemIndex);
        if (cupAssembler.CanReceivePiece(item)) {
            player.GetComponent<Inventory>().PullItem(itemIndex);
            var elementGO = GameObject.Instantiate(item.TargetPrefab, Vector3.zero, Quaternion.identity);
            cupAssembler.PutOutNextPiece(elementGO);
        }
    }

    private void OnDeactivate() {
        questCamera.m_Priority -= 2;
        player.GetComponentInChildren<Animator>(true).gameObject.SetActive(true);
        OnExit?.Invoke();
        activated = false;

        var playerInventory = player.GetComponent<Inventory>();
        playerInventory.OnItemActivated -= PlayerInventoryOnItemActivated;
    }

    private bool waitForPickUp;
    private float waitStartTime;
    private PickUpable pickUpable;

    private void OnFinish() {
        OnDeactivate();

        cupAssembler.ClearPieces();

        pickUpable = cupAssembler.CreateCupPickupable();
        waitForPickUp = true;
        waitStartTime = Time.time;
    }

    private bool rotationStage;
    private Quaternion rotationProgres;

    private void Update() {
        if (waitForPickUp) {
            if (Time.time > waitStartTime + cameraCutDuration) {
                waitForPickUp = false;
                player.ActivatePickUp(pickUpable);
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

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) {
                OnDeactivate();
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                if (activatedQuestItem != null) {
                    HandleManipulationResult();
                } else if (selector.Selected != null) {
                    if (cupAssembler.TryGetAssocicatedPieceItem(selector.Selected.gameObject, out var selectedQuestItem)) {
                        ActivateManipulationState(selectedQuestItem);
                    }
                }
            }

            if (activatedQuestItem != null) {
                var mousePointer = Camera.main.ScreenPointToRay(Input.mousePosition);
                var assemblyPlane = cupAssembler.GetAssemblyPlane();
                if (assemblyPlane.Raycast(mousePointer, out float enter)) {
                    var raycastPoint = mousePointer.GetPoint(enter);
                    var finalTranslationPoint = raycastPoint;
                    var isSnappedToAssemblyCenter = Vector3.Distance(raycastPoint, cupAssembler.GetAssemblyCenter()) < 0.3;
                    SetIsRotationStage(isSnappedToAssemblyCenter);
                    if (isSnappedToAssemblyCenter) {
                        finalTranslationPoint = cupAssembler.GetAssemblyCenter();
                    } else {
                        SetIsRotationInSpot(false);
                    }
                    activatedQuestItem.elementGO.transform.position = finalTranslationPoint;
                }

                if (rotationStage) {
                    var rotationDelta = Quaternion.AngleAxis(Input.mouseScrollDelta.y * scrollSpeed, Vector3.up);
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
        cupAssembler.SetAsseblyCenterHighlighted(true);
    }

    private void HandleManipulationResult() {
        if (activatedQuestItem.isInSpot) {
            activatedQuestItem.elementGO.GetComponent<CupQuestElement>().SetSealed();
        } else {
            activatedQuestItem.elementGO.GetComponent<CupQuestElement>().Reset();
            activatedQuestItem.elementGO.transform.position = activatedQuestItem.initialPosition;
        }

        activatedQuestItem = null;
        cupAssembler.SetAsseblyCenterHighlighted(false);

        if (cupAssembler.IsAllPiecesInSpot()) {
            OnFinish();
        }
    }

    private void SetIsRotationStage(bool rotationStage) {
        if (!this.rotationStage && rotationStage) {
            rotationProgres = activatedQuestItem.elementGO.transform.rotation;
        }
        this.rotationStage = rotationStage;
        cupAssembler.SetAsseblyCenterHighlighted(!rotationStage);
    }

    private void SetIsRotationInSpot(bool isInSpot) {
        activatedQuestItem.isInSpot = isInSpot;
        activatedQuestItem.elementGO.GetComponent<CupQuestElement>().SetIsInSpotVisuals(isInSpot);
    }

}
