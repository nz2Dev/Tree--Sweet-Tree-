using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private LayerMask groundMask;

    private AutomaticMovement movement;
    private ObjectSelector selector;

    private PickUpable pickUpTarget;

    private void Awake() {
        movement = GetComponent<AutomaticMovement>();
        selector = GetComponent<ObjectSelector>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (selector.Selected != null) {
                StartPickUp();
            } else {
                StartNavigation();
            }
        }

        UpdatePickUp();
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

    private void UpdatePickUp() {
        if (pickUpTarget != null) {
            movement.PrintDebug();
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
        pickUpTarget = null;
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
