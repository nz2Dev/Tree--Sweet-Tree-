using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {

    [SerializeField] private Transform pickUpHolder;
    [SerializeField] private AnimationCurve pickUpCurve;
    [SerializeField] private float pickingUpDuration = 0.4f;

    private AutomaticMovement movement;
    private PopUpNotifications notifications;
    private HovanetsCharacter character;
    private Inventory inventory;

    private void Awake() {
        movement = GetComponent<AutomaticMovement>();
        notifications = GetComponent<PopUpNotifications>();
        character = GetComponentInChildren<HovanetsCharacter>();
        inventory = GetComponent<Inventory>();
    }

    private void Update() {
        UpdatePickUp();
        var currentMovementSpeed = movement.GetCurrentSpeed();
        if (currentMovementSpeed > 0) {
            character.SetIsWalking(true);
            character.SetWalkMotionSpeed(currentMovementSpeed);
        } else {
            character.SetIsWalking(false);
        }
    }

    private PickUpable activePickUpable;
    private Vector3 activePickUpableStartPosition;
    private float pickUpActivationStartTime;

    public bool CanPickUpFromHere(PickUpable pickUpable) {
        return Vector3.Distance(transform.position, pickUpable.transform.position) < pickUpable.PickUpRadius;
    }

    public void CancelPickUp() {
        // todo probably should call release on pickupable
        activePickUpable = null;
    }

    public void ActivatePickUp(PickUpable pickUpable) {
        movement.StopMovement();
        activePickUpable = pickUpable;
        activePickUpableStartPosition = pickUpable.transform.position;
        pickUpActivationStartTime = Time.time;
    }

    public bool IsPickingUp() {
        return activePickUpable != null;
    }
    
    private void UpdatePickUp() {
        if (activePickUpable == null) {
            return;
        }

        var pickUpDelay = 0.3f;
        var delayEndTime = pickUpActivationStartTime + pickUpDelay;
        if (Time.time > delayEndTime) {
            var pickUpEndTime = delayEndTime + pickingUpDuration; 
            if (Time.time < pickUpEndTime) {
                var pickingUpTime = pickingUpDuration - (pickUpEndTime - Time.time);
                var pickingUpProgress = pickingUpTime / pickingUpDuration;
                var upDelta = pickUpCurve.Evaluate(pickingUpProgress) * Vector3.up;
                var objectToHands = pickUpHolder.position - activePickUpableStartPosition;

                activePickUpable.transform.position = activePickUpableStartPosition + objectToHands * pickingUpProgress + upDelta;
            } else {
                // Time > pickUpEndTime
                activePickUpable.transform.position = pickUpHolder.position;
                activePickUpable.transform.SetParent(pickUpHolder, true);
                activePickUpable = null;
            }
        }
    }

    public bool HasPickedUp() {
        return GetHandledObject() != null;
    }

    public void HandlePickedUp() {
        var pickedUp = GetHandledObject();
        if (pickedUp.name == "Bag") {
            inventory.IsWorking = true;
            // destory object from hands
            Destroy(pickedUp.gameObject);
            return;
        }

        if (!inventory.IsWorking) {
            // drop object from hands
            pickedUp.transform.SetParent(null, true);
            pickedUp.Release();

            notifications.SendNotification("Where to put it?", 2f);
        } else {
            // handle object by inventory
            inventory.Put(pickedUp);
        }
    }

    public void DropPickedUp() {
        var handledPickUpable = GetHandledObject();
        handledPickUpable.transform.SetParent(null, true);
        handledPickUpable.Release();
    }
    
    private PickUpable GetHandledObject() {
        if (pickUpHolder.transform.childCount == 0) {
            return null;
        }
        var handledObject = pickUpHolder.transform.GetChild(0);
        return handledObject == null ? null : handledObject.GetComponent<PickUpable>();
    }

    public void ActivateNavigation(Vector3 point) {
        // cancel all the rest
        CancelPickUp();        

        movement.MoveTo(point);
    }

    public float GetRemainingNavigationDistance() {
        return movement.GetRemainingDistance();
    }

    public void StopNavigation() {
        movement.StopMovement();
    }
}
