using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform pickUpHolder;
    [SerializeField] private AnimationCurve pickUpCurve;
    [SerializeField] private float pickingUpDuration = 0.4f;

    private AutomaticMovement movement;
    private ObjectSelector selector;
    private PopUpNotifications notifications;
    private Inventory inventory;

    private PickUpable pickUpTarget;
    private PickUpable pickUpDelayObject;
    private PickUpable pickUpObject;
    private float pickUpActivationTime;
    private Vector3 pickUpObjectStartPosition;
    private float pickUpObjectStartTime;

    private void Awake() {
        movement = GetComponent<AutomaticMovement>();
        selector = GetComponent<ObjectSelector>();
        notifications = GetComponent<PopUpNotifications>();
        inventory = GetComponent<Inventory>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (selector.Selected != null) {
                StartPickUp();
            } else {
                StartNavigation();
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            DropObject();
        }

        UpdatePickUpMovement();
        UpdatePickUpDelay();
        UpdatePickingUp();
    }

    private void StartPickUp() {
        pickUpTarget = selector.Selected.GetComponent<PickUpable>();
        // var playerToSelected = pickUpTarget.transform.position - transform.position;
        // var pickUpOffset = -playerToSelected.normalized * pickUpTarget.PickUpRadius;
        // var playerToStopPoint = playerToSelected + pickUpOffset;
        // var pickUpPoint = transform.position + playerToStopPoint;
        // movement.MoveTo(pickUpPoint);
        movement.MoveTo(pickUpTarget.transform.position);
    }

    private void UpdatePickUpMovement() {
        if (pickUpTarget != null) {
            if (movement.GetRemainingDistance() < pickUpTarget.PickUpRadius) {
                ActivatePickUp();
            }
        }
    }

    private void CancelPickUp() {
        pickUpTarget = null;
    }

    private void ActivatePickUp() {
        movement.StopMovement();
        pickUpDelayObject = pickUpTarget;
        pickUpActivationTime = Time.time;
        pickUpTarget = null;
    }

    private void UpdatePickUpDelay() {
        if (pickUpDelayObject != null) {
            if (pickUpActivationTime + 0.3 < Time.time) {
                StartPickingUp(pickUpDelayObject);
                pickUpDelayObject = null;
            }
        }
    }

    private void StartPickingUp(PickUpable pickUpable) {
        pickUpObject = pickUpable;
        pickUpObjectStartPosition = pickUpObject.transform.position;
        pickUpObjectStartTime = Time.time;
    }

    private void UpdatePickingUp() {
        if (pickUpObject != null) {
            if (pickUpObjectStartTime + pickingUpDuration > Time.time) {
                var pickingUpTime = Time.time - pickUpObjectStartTime;
                var pickingUpProgress = pickingUpTime / pickingUpDuration;
                var upDelta = pickUpCurve.Evaluate(pickingUpProgress) * Vector3.up;
                var objectToHands = pickUpHolder.position - pickUpObjectStartPosition;
                pickUpObject.transform.position = pickUpObjectStartPosition + objectToHands * pickingUpProgress + upDelta;
            } else {
                pickUpObject.transform.position = pickUpHolder.position;
                pickUpObject.transform.SetParent(pickUpHolder, true);
                StartPutInInvetory(pickUpObject);
                pickUpObject = null;
            }
        }
    }

    private void StartPutInInvetory(PickUpable pickUpObject) {
        if (!inventory.IsWorking) {
            DropObject();
            notifications.SendNotification("Where to put it?", 2f);
        }
    }

    private void DropObject() {
        var handledObject = pickUpHolder.transform.GetChild(0);
        handledObject.transform.SetParent(null, true);
        handledObject.GetComponent<PickUpable>().Release();
    }

    private void StartNavigation() {
        // cancel all the rest
        CancelPickUp();        

        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out var hit, groundMask)) {
            movement.MoveTo(hit.point);
        }
    }
}
