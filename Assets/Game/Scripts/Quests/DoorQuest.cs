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

    private bool transportationSelection;
    private DoorQuestElement transported;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float startTime;
    private DoorQuestZone destination;
    private bool transporting;

    private void Update() {
        if (active) {
            if (Input.GetMouseButtonDown(1)) {
                Deactivate();
            }

            if (transportationSelection) {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                    TryChooseTransportationZone();
                }
            }

            if (!transportationSelection) {
                if (!transporting && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                    TrySetTransportationElement();
                }
            }

            if (transporting) {
                var duration = 1.0f;
                var endTime = startTime + duration;
                if (Time.time < endTime) {
                    var progress = (Time.time - startTime) / duration;
                    transported.transform.position = Vector3.Lerp(startPosition, destination.transform.position, progress);
                    transported.transform.rotation = Quaternion.Lerp(startRotation, destination.transform.rotation, progress);
                } else {
                    destination.SetResident(transported);
                    transporting = false;
                    OnTransportationFinished();
                }
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

    private void OnTransportationFinished() {
        if (transported.gameObject == placedElementGO && destination != dynamicZone) {
            if (IsAllStaticElementsInPlace()) {
                transported.gameObject.SetActive(false);
                Finish();
            } else {
                StartTransportationTo(dynamicZone);
            }
        }
    }

    private void TryChooseTransportationZone() {
        transportationSelection = false;

        transported.GetComponent<SelectableObject>().StopHighlighting();
        selector.CancelOverrideMask();

        if (selector.Selected != null && selector.Selected.TryGetComponent<DoorQuestZone>(out var zone)) {
            if (!zone.HasResident && (zone.IsSiblingTo(transported.Host) || transported.gameObject == placedElementGO)) {
                StartTransportationTo(zone);
            }
        }
    }

    private void TrySetTransportationElement() {
        if (selector.Selected != null && selector.Selected.TryGetComponent<DoorQuestElement>(out var element)) {
            selector.Selected.Highlight();
            selector.OverrideMask(transportationMask);
            transported = element;
            transportationSelection = true;
        }
    }

    private void StartTransportationTo(DoorQuestZone zone) {
        destination = zone;
        startTime = Time.time;
        startPosition = transported.transform.position;
        startRotation = transported.transform.rotation;
        transporting = true;
        transported.SetTransported();
    }
}
