using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectSelector : MonoBehaviour {

    [SerializeField] private LayerMask selectableObjectsMask;

    private SelectableObject selectedObject;
    private LayerMask overrideMask;
    private bool overridingMask;

    public SelectableObject Selected => selectedObject;

    private void Update() {
        UpdateSelection();
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            if (selectedObject != null) {
                selectedObject.Click();
            }
        }
    }

    public void OverrideMask(LayerMask overrideMask) {
        this.overrideMask = overrideMask;
        overridingMask = true;
    }

    public void CancelOverrideMask() {
        this.overridingMask = false;
    }

    private void UpdateSelection() {
        if (Cursor.visible && TryRaycastSelectable(out var raycasted)) {
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
        
        var mask = overridingMask ? overrideMask : selectableObjectsMask;
        if (Physics.Raycast(mouseRay, out var hitInfo, 100, mask)) {
            raycastedSelectable = hitInfo.collider.GetComponentInParent<SelectableObject>();
        } else {
            raycastedSelectable = null;
        }
        
        return raycastedSelectable != null;
    }

}
