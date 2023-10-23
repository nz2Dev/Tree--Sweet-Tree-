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
    [SerializeField] private LayerMask zoneMask;
    [SerializeField] private DoorStates door;
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

    private void OnQuestFinished() {
        active = false;
        vcam.m_Priority -= 2;
        door.SetState(DoorStates.State.Stationar);

        player.GetComponent<Inventory>().OnItemActivated -= InventoryOnItemActivated;
        placedElementGO.SetActive(false);
        Debug.Log("Quest Finished!");
    }

    private DoorQuestElement chosenElement;
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
            if (TrySelectZoneFor(chosenElement, out var zone)) {
                StartTransportation(chosenElement, zone);
            }
            selector.CancelOverrideMask();
            UnsetChosenElement();
        } else if (TrySelectElement(out var element)) {
            selector.OverrideMask(zoneMask);
            ChooseElement(element);
        }
    }

    private bool TrySelectZoneFor(DoorQuestElement element, out DoorQuestZone selectedZone) {
        selectedZone = null;
        if (selector.Selected != null && selector.Selected.TryGetComponent<DoorQuestZone>(out var zone)) {
            if (!zone.HasResident && (zone.IsSiblingTo(element.Host) || element.gameObject == placedElementGO)) {
                selectedZone = zone;
            }
        }
        return selectedZone != null;
    }

    private bool IsElementChosen() {
        return chosenElement != null;
    }

    private bool TrySelectElement(out DoorQuestElement selectedElement) {
        selectedElement = null;
        if (!transporting && selector.Selected != null && selector.Selected.TryGetComponent<DoorQuestElement>(out var element)) {
            selectedElement = element;
        }
        return selectedElement != null;
    }

    private void ChooseElement(DoorQuestElement element) {
        chosenElement = element;
        chosenElement.GetComponent<SelectableObject>().Highlight();
    }

    private void UnsetChosenElement() {
        chosenElement.GetComponent<SelectableObject>().StopHighlighting();
        chosenElement = null; 
    }

    private void StartTransportation(DoorQuestElement element, DoorQuestZone zone) {
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
                OnQuestFinished();
            } else {
                StartTransportation(transportationElement, dynamicZone);
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