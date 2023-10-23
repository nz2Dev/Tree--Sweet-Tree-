using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorQuest : MonoBehaviour {
    
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private ObjectSelector selector;
    [SerializeField] private LayerMask transportationMask;
    [SerializeField] private DoorStates door;
    [SerializeField] private Transform inventoryItemPlacement;
    [SerializeField] private DoorQuestElement[] questElements;
    [SerializeField] private DoorQuestZone dynamicZone;
    [SerializeField] private Player player;

    private bool active = false;
    private GameObject placedElementGO;

    private void Awake() {
        vcam.m_Priority = 9;
    }

    public void Activate() {
        active = true;
        vcam.m_Priority += 2;
        door.SetState(DoorStates.State.Quest);

        player.GetComponent<Inventory>().OnItemActivated += InventoryOnItemActivated;
    }

    public void Deactivate() {
        active = false;
        vcam.m_Priority -= 2;
        door.SetState(DoorStates.State.Activator);

        player.GetComponent<Inventory>().OnItemActivated -= InventoryOnItemActivated;
    }

    private void InventoryOnItemActivated(Item item) {
        var activatedItemGO = GameObject.Instantiate(item.prefab, Vector3.zero, Quaternion.identity);
        placedElementGO = activatedItemGO;
        dynamicZone.SetResident(activatedItemGO.GetComponent<DoorQuestElement>());
    }

    private void Finish() {
        active = false;
        vcam.m_Priority -= 2;
        door.SetState(DoorStates.State.Stationar);

        player.GetComponent<Inventory>().OnItemActivated -= InventoryOnItemActivated;
        Debug.Log("Quest Finished!");
    }

    private DoorQuestElement selectedTransportationElement;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float startTime;
    private DoorQuestZone destination;
    private DoorQuestElement transportationElement;
    private bool transporting;

    private void Update() {
        if (active) {
            if (Input.GetMouseButtonDown(1)) {
                Deactivate();
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                HandleClick();
            }

            if (transporting) {
                var duration = 1.0f;
                var endTime = startTime + duration;
                if (Time.time < endTime) {
                    var progress = (Time.time - startTime) / duration;
                    transportationElement.transform.position = Vector3.Lerp(startPosition, destination.transform.position, progress);
                    transportationElement.transform.rotation = Quaternion.Lerp(startRotation, destination.transform.rotation, progress);
                } else {
                    destination.SetResident(transportationElement);
                    transporting = false;
                    OnTransportationFinished();
                }
            }
        }
    }

    private void HandleClick() {
        if (IsElementChosen()) {
            TryChooseZone(selectedTransportationElement);
            UnsetChosenElement();
        } else if (TrySelectElement(out var element)) {
            ChooseElement(element);
        }
    }

    private void TryChooseZone(DoorQuestElement transportationElement) {
        if (selector.Selected != null && selector.Selected.TryGetComponent<DoorQuestZone>(out var zone)) {
            if (!zone.HasResident && (zone.IsSiblingTo(transportationElement.Host) || transportationElement.gameObject == placedElementGO)) {
                StartTransportationTo(transportationElement, zone);
            }
        }
    }

    private bool IsElementChosen() {
        return selectedTransportationElement != null;
    }

    private bool TrySelectElement(out DoorQuestElement selectedElement) {
        selectedElement = null;
        if (!transporting && selector.Selected != null && selector.Selected.TryGetComponent<DoorQuestElement>(out var element)) {
            selectedElement = element;
        }
        return selectedElement != null;
    }

    private void ChooseElement(DoorQuestElement element) {
        selector.OverrideMask(transportationMask);
        selectedTransportationElement = element;
        selectedTransportationElement.GetComponent<SelectableObject>().Highlight();
    }

    private void UnsetChosenElement() {
        selector.CancelOverrideMask();
        selectedTransportationElement.GetComponent<SelectableObject>().StopHighlighting();
        selectedTransportationElement = null; 
    }

    private void StartTransportationTo(DoorQuestElement element, DoorQuestZone zone) {
        transporting = true;
        
        destination = zone;
        startTime = Time.time;
        startPosition = element.transform.position;
        startRotation = element.transform.rotation;

        transportationElement = element;
        element.SetTransported();
    }

    private void OnTransportationFinished() {
        if (transportationElement.gameObject == placedElementGO && destination != dynamicZone) {
            if (IsAllStaticElementsInPlace()) {
                transportationElement.gameObject.SetActive(false);
                Finish();
            } else {
                StartTransportationTo(transportationElement, dynamicZone);
            }
        }
    }

    private bool IsAllStaticElementsInPlace() {
        foreach (var questElement in questElements) {
            if (!questElement.IsOnDesiredHost) {
                return false;
            }
        }
        return true;
    }
}