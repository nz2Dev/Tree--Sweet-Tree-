using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSelector : MonoBehaviour {

    [SerializeField] private LayerMask selectableObjectsMask;

    private SelectableObject selectedObject;

    public SelectableObject Selected => selectedObject;

    private void Update() {
        UpdateSelection();
    }

    private void UpdateSelection() {
        if (TryRaycastSelectable(out var raycasted)) {
            if (raycasted != selectedObject) {
                MakeSelected(raycasted);
            }
        } else {
            UnselectCurrent();
        }
    }

    private void MakeSelected(SelectableObject selectable) {
        UnselectCurrent();
        Select(selectable);
    }

    private void UnselectCurrent() {
        if (selectedObject != null) {
            selectedObject.OnUnselected();
        }
        selectedObject = null;
    }

    private void Select(SelectableObject selectable) {
        selectedObject = selectable;
        selectedObject.OnSelected();
    }

    private bool TryRaycastSelectable(out SelectableObject raycastedSelectable) {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(mouseRay, out var hitInfo, 100, selectableObjectsMask)) {
            raycastedSelectable = hitInfo.collider.GetComponentInParent<SelectableObject>();
        } else {
            raycastedSelectable = null;
        }
        
        return raycastedSelectable != null;
    }

}
