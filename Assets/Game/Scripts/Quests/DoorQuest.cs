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

    private void Start() {
        if (door.IsQuestState()) {
            Activate();
        }
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
                    transported.GetComponent<SelectableObject>().StopHighlighting();
                    var selectedSurface = selector.Selected;
                    selector.CancelOverrideMask();
                    transportationSelection = false;
                    
                    if (selectedSurface != null && selectedSurface.TryGetComponent<DoorQuestZone>(out var zone) && !zone.HasResident) {                        
                        startTime = Time.time;
                        startPosition = transported.transform.position;
                        startRotation = transported.transform.rotation;
                        destination = zone;
                        transporting = true;
                        transported.SetTransported();
                    }
                }
            }

            if (!transportationSelection) {
                if (!transporting && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && selector.Selected != null) {
                    if (selector.Selected.TryGetComponent<DoorQuestElement>(out var element)) {
                        selector.Selected.Highlight();
                        selector.OverrideMask(transportationMask);
                        transportationSelection = true;
                        transported = element;
                    }
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

                    if (transported.gameObject == placedElementGO && destination != dynamicZone) {
                        bool allStaticElementsInPlace = true;
                        
                        foreach (var questElement in questElements) {
                            if (!questElement.IsOnDesiredHost) {
                                allStaticElementsInPlace = false;
                                break;
                            }
                        }

                        if (allStaticElementsInPlace) {
                            transported.gameObject.SetActive(false);
                            Finish();
                        } else {
                            startTime = Time.time;
                            startPosition = transported.transform.position;
                            startRotation = transported.transform.rotation;
                            destination = dynamicZone;
                            transporting = true;
                            transported.SetTransported();
                        }
                    }
                }
            }
        }
    }

}
