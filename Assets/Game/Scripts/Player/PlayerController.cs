using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public interface IPlayerActivity {
    bool IsFinished { get; }
    void Begin(Player player);
    void Update(Player player);
    void Cancel(Player player);
}

public class PlayerController : MonoBehaviour {
    
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Vector2 cursorHotSpot = new Vector2(16, 16);
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D manipulationCursor;
    [SerializeField] private Texture2D navigationCursor;

    private ObjectSelector selector;
    private Player player;

    private IPlayerActivity currentActivity;

    private bool raycastForNavigation;
    private Vector3 raycastPoint;

    private void Awake() {
        player = GetComponent<Player>();
        selector = GetComponent<ObjectSelector>();
    }

    private void Update() {
        RaycastNavigationPoint();
        UpdateCursorIcon();

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            if (selector.Selected != null) {
                if (selector.Selected.TryGetComponent<PickUpable>(out var selectedPickUp)) {
                    ExecuteActivity(new PlayerPickUpObjectActivity(selectedPickUp, onCancel: () => {
                        selector.CancelLastHighlight();
                    }));
                    selector.HighlightSelection();
                } else if (selector.Selected.TryGetComponent<ActivationObject>(out var activationObject)) {
                    ExecuteActivity(new PlayerActivateObjectActivity(activationObject, onCancel: () => {
                        selector.CancelLastHighlight();
                    }));
                    selector.HighlightSelection();
                }
            } else if (raycastForNavigation) {
                ExecuteActivity(new PlayerNavigateToPointActivity(raycastPoint));
            }
        }

        if (currentActivity != null) {
            currentActivity.Update(player);

            if (currentActivity.IsFinished) {
                currentActivity.Cancel(player);
                currentActivity = null;
            }
        }
    }

    private void RaycastNavigationPoint() {
        raycastForNavigation = false;
        
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out var hit, 100f, groundMask)) {
            if (NavMesh.SamplePosition(hit.point, out var navMeshHit, 0.5f, NavMesh.AllAreas)) {
                raycastPoint = hit.point;
                raycastForNavigation = true;
            }
        }
    }

    private void UpdateCursorIcon() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            Cursor.SetCursor(defaultCursor, cursorHotSpot, CursorMode.Auto);
            return;
        }

        Cursor.SetCursor(defaultCursor, cursorHotSpot, CursorMode.Auto);
        if (selector.Selected != null) {
            if (currentActivity is not PlayerPickUpObjectActivity pickUpActivity 
                || pickUpActivity.TargetPickUpable.gameObject != selector.Selected.gameObject) {
                Cursor.SetCursor(manipulationCursor, cursorHotSpot, CursorMode.Auto);                
            }
        } else if (raycastForNavigation) {
            Cursor.SetCursor(navigationCursor, cursorHotSpot, CursorMode.Auto);
        }
    }

    private void ExecuteActivity(IPlayerActivity activity) {
        if (currentActivity != null) {
            currentActivity.Cancel(player);
        }

        currentActivity = activity;
        currentActivity.Begin(player);
    }

}
