using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour {

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

    private void Start() {
        character.SetBagEquiped(inventory.IsWorking);
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
    private Transform pickUpDestination;
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
        pickUpDestination = pickUpable.name == "Bag" ? character.HandsLocation : character.BagLocation;
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
                var objectToHands = pickUpDestination.position - activePickUpableStartPosition;

                activePickUpable.transform.position = activePickUpableStartPosition + objectToHands * pickingUpProgress + upDelta;
            } else {
                // Time > pickUpEndTime
                activePickUpable.transform.position = pickUpDestination.position;
                activePickUpable.transform.SetParent(pickUpDestination, true);
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
            character.SetBagEquiped(inventory.IsWorking);
            // destory object from hands
            pickedUp.DestroySelf(consumed: true);
            return;
        }

        if (!inventory.IsWorking) {
            // drop object from hands
            pickedUp.transform.SetParent(null, true);
            pickedUp.Release();

            notifications.SendNotification("Where to put it?", 2f);
        } else {
            // handle object by inventory
            inventory.Put(pickedUp.InventoryItem);
            pickedUp.DestroySelf(consumed: true);
        }
    }

    public void DropPickedUp() {
        var handledPickUpable = GetHandledObject();
        handledPickUpable.transform.SetParent(null, true);
        handledPickUpable.Release();
    }
    
    private PickUpable GetHandledObject() {
        if (pickUpDestination.transform.childCount == 0) {
            return null;
        }
        var handledObject = pickUpDestination.transform.GetChild(0);
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
