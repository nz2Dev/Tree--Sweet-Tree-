using System;
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
    
    [SerializeField] private LayerMask navigationMap;
    [SerializeField] private Vector2 cursorHotSpot = new Vector2(16, 16);
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D manipulationCursor;
    [SerializeField] private Texture2D navigationCursor;
    [SerializeField] private ObjectSelector selector;
    [SerializeField] private CursorMode cursorMode;

    private Player player;

    private Queue<IPlayerActivity> activitiesQueue;
    private IPlayerActivity currentActivity;
    private PlayerTransportableObjectController transportationObjectController;

    private bool raycastForNavigation;
    private Vector3 raycastPoint;
    private bool raycastForJump;
    private JumpPlatform raycastedPlatform;
    private bool raycastForClimbing;
    private ClimbingNode raycastedClimbingNode;

    private void Awake() {
        player = GetComponent<Player>();
        activitiesQueue = new Queue<IPlayerActivity>();
        transportationObjectController = new PlayerTransportableObjectController(selector);
    }

    private void Start() {
        player.Inventory.RegisterItemActivationController(PlayerInventoryOnItemActivated);
    }

    private void PlayerInventoryOnItemActivated(int itemIndex) {
        player.DropInventoryItem(itemIndex);
    }

    private void Update() {
        RaycastNavigationPoint();
        UpdateCursorIcon();

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            if (selector.Selected != null) {
                if (selector.Selected.TryGetComponent<PickUpable>(out var selectedPickUp)) {
                    ExecuteActivity(new PlayerPickUpObjectActivity(selectedPickUp));
                } else if (selector.Selected.TryGetComponent<ActivationObject>(out var selectedActivationObject)) {
                    ExecuteActivity(new PlayerActivateObjectActivity(selectedActivationObject));
                }
            } else if (raycastForNavigation) {
                ExecuteActivity(new PlayerNavigateToPointActivity(raycastPoint));
            } else if (raycastForJump) {
                ExecuteActivity(new PlayerNavigateToJumpActivity(raycastedPlatform));
            } else if (raycastForClimbing) {
                if (raycastedClimbingNode.CanClimbeOnto) {
                    if (raycastedClimbingNode.TryGetClimbingConnector(player.transform.position, out var connector)) {
                        EnqueueActivity(new PlayerNavigateToJumpActivity(connector.hopPlatform));
                        EnqueueActivity(new PlayerClimbMovePlatformActivity(connector.movePlatform));
                        EnqueueActivity(new PlayerNavigateToJumpActivity(connector.dropPlatform));
                    } else {
                        player.ActivateJump(null);
                    }
                } else {
                    player.ReceiveNotification(raycastedClimbingNode.EnablingSuggestion);
                }
            }
        }

        if (currentActivity != null) {
            currentActivity.Update(player);

            if (currentActivity.IsFinished) {
                currentActivity.Cancel(player);
                currentActivity = null;
            }
        }

        if (player.GrabbedObject != null && !transportationObjectController.IsActivated) { 
            transportationObjectController.OnActivated(player, player.GrabbedObject.OverviewCam);
        }
        if (player.GrabbedObject == null && transportationObjectController.IsActivated) {
            transportationObjectController.OnDeactivated();
        }
        if (transportationObjectController.IsActivated) {
            transportationObjectController.OnUpdate();
        }

        if (currentActivity == null) {
            if (activitiesQueue.TryDequeue(out var nextActivity)) {
                ExecuteActivity(nextActivity);
            }
        }
    }

    private void RaycastNavigationPoint() {
        raycastForNavigation = false;
        raycastForJump = false;
        raycastForClimbing = false;
        
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out var hit, 100f, navigationMap)) {
            if (hit.collider.TryGetComponent<JumpPlatform>(out var jumpPlatform)) {
                raycastedPlatform = jumpPlatform;
                raycastForJump = true;
            } else if (hit.collider.TryGetComponent<ClimbingNode>(out var climbingNode)) {
                raycastedClimbingNode = climbingNode;
                raycastForClimbing = true; 
            } else {    
                raycastPoint = hit.point;
                raycastForNavigation = true;
            }
        }
    }

    private void UpdateCursorIcon() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            Cursor.SetCursor(defaultCursor, cursorHotSpot, cursorMode);
            return;
        }

        Cursor.SetCursor(defaultCursor, cursorHotSpot, cursorMode);
        if (selector.Selected != null) {
            if (currentActivity is not PlayerPickUpObjectActivity pickUpActivity 
                || pickUpActivity.TargetPickUpable.gameObject != selector.Selected.gameObject) {
                Cursor.SetCursor(manipulationCursor, cursorHotSpot, cursorMode);                
            }
        } else if (raycastForNavigation || raycastForJump || raycastForClimbing) {
            Cursor.SetCursor(navigationCursor, cursorHotSpot, cursorMode);
        }
    }

    private void ExecuteActivity(IPlayerActivity activity) {
        if (currentActivity != null) {
            currentActivity.Cancel(player);
        }

        currentActivity = activity;
        currentActivity.Begin(player);
    }

    private void EnqueueActivity(IPlayerActivity activity) {
        activitiesQueue.Enqueue(activity);
    }

}
