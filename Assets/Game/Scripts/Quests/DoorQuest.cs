using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorQuest : MonoBehaviour {
    
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private ObjectSelector selector;
    [SerializeField] private LayerMask transportationMask;
    [SerializeField] private DoorStates door;

    private bool active = false;

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
    }

    public void Deactivate() {
        active = false;
        vcam.m_Priority -= 2;
        door.SetState(DoorStates.State.Activator);
    }

    private bool transportationSelection;
    private DoorQuestElement transported;
    private Vector3 startPosition;
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
                        destination = zone;
                        transporting = true;
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
                        transported.SetTransported();
                    }
                }
            }

            if (transporting) {
                var duration = 1.0f;
                var endTime = startTime + duration;
                if (Time.time < endTime) {
                    var progress = (Time.time - startTime) / duration;
                    transported.transform.position = Vector3.Lerp(startPosition, destination.transform.position, progress);
                } else {
                    destination.SetResident(transported);
                    transporting = false;
                }
            }
        }
    }

}
