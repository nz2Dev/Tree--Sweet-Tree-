using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSelector : MonoBehaviour {

    [SerializeField] private LayerMask selectableObjectsMask;

    private SelectableObject selectedObject;

    private void Update() {
        if (TryRaycastNextSelectableObject(out var raycastedSelectable)) {
            var isRaycastedNewSelectable = raycastedSelectable != selectedObject;
            if (isRaycastedNewSelectable) {
                if (selectedObject != null) {
                    selectedObject.MarkUnselected();
                }
                
                selectedObject = raycastedSelectable;
                selectedObject.MarkSelected();
            }
        } else {
            if (selectedObject != null) {
                selectedObject.MarkUnselected();
                selectedObject = null;
            }
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
