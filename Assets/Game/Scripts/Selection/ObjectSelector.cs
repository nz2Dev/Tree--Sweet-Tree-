using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSelector : MonoBehaviour {

    [SerializeField] private LayerMask selectableObjectsMask;

    private SelectableObject selectedObject;
    private SelectableObject highlightedObject;

    public SelectableObject Selected => selectedObject;

    private void Update() {
        UpdateSelection();
    }

    public void HighlightSelection() {
        if (selectedObject != null) {
            if (highlightedObject != null) {
                highlightedObject.StopHighlighting();
                highlightedObject = null;
            }
            
            highlightedObject = selectedObject;
            highlightedObject.Highlight();
        }
    }

    public void CancelLastHighlight() {
        if (highlightedObject != null) {
            highlightedObject.StopHighlighting();
            highlightedObject = null;
        }
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
