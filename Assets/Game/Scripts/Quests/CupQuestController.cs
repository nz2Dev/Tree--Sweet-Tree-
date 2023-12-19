using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CupQuestController : MonoBehaviour {

    [SerializeField] private ObjectSelector selector;
    [SerializeField] private CinemachineVirtualCamera questCamera;   
    [SerializeField] private CupAssembler cupAssembler; 
    [SerializeField] private GameObject rotationUI;
    [SerializeField] private RotationButton clockwiseButton;
    [SerializeField] private RotationButton contrClockwiseButton;
    [SerializeField] private float rotationHoldSpeedDegPerSec = 45;
    [SerializeField] private GameObject applyCursorArea;
    [SerializeField] private Player player;
    [SerializeField] private float cameraCutDuration = 0.9f;
    [SerializeField] private float scrollSpeed = 4f;

    private bool activated;
    private float activationStartTime;
    private bool hasFinished;

    public bool HasFinished => hasFinished;

    public event Action OnExit;

    private void Start() {
        rotationUI.SetActive(false);
        applyCursorArea.SetActive(false);
    }

    public void OnActivated() {
        activationStartTime = Time.time;
        questCamera.m_Priority += 2;
        activated = true;
        
        var playerInventory = player.GetComponent<Inventory>();
        playerInventory.RegisterItemActivationController(PlayerInventoryOnItemActivated);
    }

    private void PlayerInventoryOnItemActivated(int itemIndex) {
        var item = player.GetComponent<Inventory>().GetItem(itemIndex);
        if (cupAssembler.CanReceivePiece(item)) {
            var activatedInventoryItem = player.GetComponent<Inventory>().PullItem(itemIndex);
            cupAssembler.DiscoverNextPiece(activatedInventoryItem);
        }
    }

    private void OnDeactivate() {
        questCamera.m_Priority -= 2;
        player.GetComponentInChildren<Animator>(true).gameObject.SetActive(true);
        OnExit?.Invoke();
        activated = false;

        var playerInventory = player.GetComponent<Inventory>();
        playerInventory.UnregisterItemActivationController(PlayerInventoryOnItemActivated);

        cupAssembler.TurnOffIndication();
    }

    private bool waitForPickUp;
    private float waitStartTime;
    private PickUpable pickUpable;

    private void OnFinish() {
        hasFinished = true;

        OnDeactivate();

        cupAssembler.ClearPieces();

        pickUpable = cupAssembler.CreateCupPickupable();
        waitForPickUp = true;
        waitStartTime = Time.time;
    }

    private void Update() {
        if (waitForPickUp) {
            if (Time.time > waitStartTime + cameraCutDuration) {
                waitForPickUp = false;
                player.ActivatePickUp(pickUpable);
            }
        }
        
        if (activated) {
            if (Application.isEditor && Input.GetKeyDown(KeyCode.F)) {
                OnFinish();
                return;
            }

            if (Time.time > activationStartTime + cameraCutDuration /*camera cut transition time*/ ) {
                player.GetComponentInChildren<Animator>(true).gameObject.SetActive(false);
                cupAssembler.TurnOnIndication();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) {
                OnDeactivate();
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                if (cupAssembler.IsQuestElementActivated()) {
                    cupAssembler.HandleManipulationResult();
                    applyCursorArea.SetActive(false);
                    
                    if (cupAssembler.IsAllPiecesInSpot()) {
                        OnFinish();
                    }
                } else if (selector.Selected != null) {
                    if (cupAssembler.TryDetectActivatablePieceElement(selector.Selected.gameObject, out var detectedQuestElement)) {
                        cupAssembler.ActivateManipulationState(detectedQuestElement);
                        applyCursorArea.SetActive(true);
                    }
                }
            }

            rotationUI.SetActive(cupAssembler.IsQuestElementActivated());

            if (cupAssembler.IsQuestElementActivated()) {
                if (cupAssembler.IsRotationStage()) {
                    var rotationInput = clockwiseButton.IsHolding ? 1 : contrClockwiseButton.IsHolding ? -1 : 0;
                    cupAssembler.RotateManipulated(Time.deltaTime * rotationHoldSpeedDegPerSec * rotationInput);
                }
            }
        }
    }

    private void HandleMovement() {
        var mousePointer = Camera.main.ScreenPointToRay(Input.mousePosition);
        var assemblyPlane = cupAssembler.GetAssemblyPlane();
        if (assemblyPlane.Raycast(mousePointer, out float enter)) {
            var raycastPoint = mousePointer.GetPoint(enter);
            var finalTranslationPoint = raycastPoint;
            var isSnappedToAssemblyCenter = Vector3.Distance(raycastPoint, cupAssembler.GetAssemblyCenter()) < 0.3;
            cupAssembler.SetIsRotationStage(isSnappedToAssemblyCenter);
            rotationUI.SetActive(isSnappedToAssemblyCenter);
            
            if (isSnappedToAssemblyCenter) {
                finalTranslationPoint = cupAssembler.GetAssemblyCenter();
            } else {
                cupAssembler.ResetManipulatedRotationInSpot();
            }

            cupAssembler.MoveManipulated(finalTranslationPoint);
        }
    }

}
