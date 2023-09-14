using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSelector : MonoBehaviour {

    [SerializeField] private LayerMask selectableObjectsMask;
    [SerializeField] private Texture2D defaultCursor;

    private SelectableObject selectedObject;
    private bool selectionLocked;

    public GameObject Selected => selectedObject == null ? null : selectedObject.gameObject;

    private void Update() {
        UpdateSelection();
    }

    public void LockSelection() {
        if (selectedObject == null) {
            Debug.LogWarning("Selected Object is null!");
        }
        selectionLocked = true;
    }

    public void UnlockSelection() {
        selectionLocked = false;
    }

    private void UpdateSelection() {
        if (selectionLocked) {
            return;
        }

        if (TryRaycastNextSelectableObject(out var raycastedSelectable)) {
            var isRaycastedNewSelectable = raycastedSelectable != selectedObject;
            if (isRaycastedNewSelectable) {
                if (selectedObject != null) {
                    selectedObject.MarkUnselected();
                }
                
                selectedObject = raycastedSelectable;
                selectedObject.MarkSelected();
                UpdateCursorIcon();
            }
        } else {
            if (selectedObject != null) {
                selectedObject.MarkUnselected();
                selectedObject = null;
                UpdateCursorIcon();
            }
        }
    }

    private void UpdateCursorIcon() {
        if (selectedObject == null) {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        } else {
            Cursor.SetCursor(selectedObject.SelectionCursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }

    private bool TryRaycastNextSelectableObject(out SelectableObject raycastedSelectable) {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(mouseRay, out var hitInfo, 100, selectableObjectsMask)) {
            raycastedSelectable = hitInfo.collider.GetComponentInParent<SelectableObject>();
        } else {
            raycastedSelectable = null;
        }
        
        return raycastedSelectable != null;
    }

}
