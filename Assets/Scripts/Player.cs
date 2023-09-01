using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private LayerMask groundMask;

    private AutomaticMovement movement;
    private ObjectSelector selector;

    private void Awake() {
        movement = GetComponent<AutomaticMovement>();
        selector = GetComponent<ObjectSelector>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (selector.Selected != null) {
                PickUpSelected();
            } else {
                MoveToRaycasted();
            }
        }
    }

    private void PickUpSelected() {
        // selector.LockSelection();
        movement.MoveTo(selector.Selected.transform.position);
    }

    private void MoveToRaycasted() {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out var hit, groundMask)) {
            movement.MoveTo(hit.point);
        }
    }
}
