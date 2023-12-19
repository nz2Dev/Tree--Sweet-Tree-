using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DoorQuest : MonoBehaviour {
    
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private ObjectSelector selector;
    [SerializeField] private LayerMask zoneMask;
    [SerializeField] private DoorStates door;
    [SerializeField] private DoorQuestElement[] questElements;
    [SerializeField] private DoorQuestZone dynamicZone;
    [SerializeField] private Player player;
    [SerializeField] private Sprite questPieceIcon;
    [SerializeField] private UnityEvent OnQuestFinishedEvent;
    [SerializeField] private float transportationDuration = 0.5f;
    [SerializeField] private AudioSource soundFXSource;
    [SerializeField] private AudioClip elementMoveSFX;

    private bool active = false;
    private GameObject placedElementGO;

    private void Awake() {
        vcam.m_Priority = 9;
    }

    public void Activate() {
        active = true;
        vcam.m_Priority += 2;
        door.SetState(DoorStates.State.Quest);
        player.HideCharacterDelayed(0.9f); // camera cut duration

        player.Inventory.RegisterItemActivationController(InventoryOnItemActivated);
    }

    public void Deactivate() {
        active = false;
        vcam.m_Priority -= 2;
        door.SetState(DoorStates.State.Activator);
        player.ShowCharacterDelayed(0.9f); // camera cut duration

        player.Inventory.UnregisterItemActivationController(InventoryOnItemActivated);
    }

    private void InventoryOnItemActivated(int itemIndex) {
        var inventory = player.GetComponent<Inventory>();
        if (inventory.GetItem(itemIndex).Icon != questPieceIcon) {
            return;
        }

        var item = inventory.PullItem(itemIndex);
        var activatedItemGO = GameObject.Instantiate(item.TargetPrefab, Vector3.zero, Quaternion.identity);
        placedElementGO = activatedItemGO;
        dynamicZone.SetResident(activatedItemGO.GetComponent<DoorQuestElement>());
    }

    private void OnQuestFinished() {
        active = false;
        vcam.m_Priority -= 2;
        door.SetState(DoorStates.State.Stationar);
        player.ShowCharacterDelayed(0.9f); // camera cut duration

        player.Inventory.UnregisterItemActivationController(InventoryOnItemActivated);
        
        if (OnQuestFinishedEvent != null) {
            OnQuestFinishedEvent.Invoke();
        }
    }

    private DoorQuestElement chosenElement;
    private DoorQuestZone transportationDestination;
    private DoorQuestElement transportationElement;
    private TweenState transportationTweenState;

    private void Update() {
        if (active) {
            if (Application.isEditor && Input.GetKeyDown(KeyCode.F)) {
                OnQuestFinished();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) {
                Deactivate();
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                HandleClick();
            }
        }

        UpdateTransportation();
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
        if (!transportationTweenState.active && selector.Selected != null && selector.Selected.TryGetComponent<DoorQuestElement>(out var element)) {
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
        transportationDestination = zone;
        transportationElement = element;
        transportationTweenState = TweenUtils.StartTween(element.transform, zone.transform, transportationDuration);
        element.SetTransported();
        soundFXSource.pitch = 1.5f + Random.Range(-0.1f, 0.05f);
        soundFXSource.PlayOneShot(elementMoveSFX);
    }

    private void UpdateTransportation() {
        if (TweenUtils.TryFinishTween(ref transportationTweenState)) {
            transportationDestination.SetResident(transportationElement);
            OnTransportationFinished();
        }
    }

    private void OnTransportationFinished() {
        if (transportationElement.gameObject == placedElementGO && transportationDestination != dynamicZone) {
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