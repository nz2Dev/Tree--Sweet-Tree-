using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class Player : MonoBehaviour {
    
    [SerializeField] private Suggestion inventorySuggestion;
    [SerializeField] private AnimationCurve pickUpCurve;
    [SerializeField] private float pickingUpDuration = 0.4f;
    [SerializeField] private float jumpMaxHight = 1f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private PlayableDirector jumpDirector;

    private NavMeshAgent navMeshAgent;
    private PopUpNotifications notifications;
    private HovanetsCharacter character;
    private Inventory inventory;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoTraverseOffMeshLink = false;
        navMeshAgent.updateRotation = false;

        notifications = GetComponent<PopUpNotifications>();
        character = GetComponentInChildren<HovanetsCharacter>();
        inventory = GetComponent<Inventory>();
    }

    private void Start() {
        character.SetBagEquiped(inventory.IsWorking);
    }

    private void Update() {
        UpdatePickUp();
        UpdateNavigation();
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
        navMeshAgent.ExtResetDestination();

        activePickUpable = pickUpable;
        activePickUpableStartPosition = pickUpable.transform.position;
        pickUpActivationStartTime = Time.time;
        
        if (pickUpable.name == "Bag" || !inventory.IsWorking) {
            pickUpDestination = character.HandsLocation;
        } else {
            pickUpDestination = character.BagLocation;
        }
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

            notifications.SendNotification(inventorySuggestion);
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

        navMeshAgent.destination = point;
    }

    public float GetRemainingNavigationDistance() {
        return navMeshAgent.ExtGetRemainingDistanceAnyState();
    }

    public void UpdateNavigation() {
        if (navMeshAgent.isOnOffMeshLink && !jumpStarted) {
            jumpStarted = true;
            jumpStartTime = Time.time;   
            jumpStartPosition = transform.position;
            character.PlayJump();
        }

        if (jumpStarted) {
            UpdateLinkJump();
        } else {
            UpdateMovement();
        }
    }

    private void UpdateMovement() {
        var currentMovementSpeed = navMeshAgent.ExtGetCurrentSpeed();
        if (currentMovementSpeed > 0) {
            character.SetIsWalking(true);
            character.SetWalkMotionSpeed(currentMovementSpeed);

            transform.rotation = Quaternion.Lerp(transform.rotation, 
                    Quaternion.LookRotation(navMeshAgent.velocity, Vector3.up), 
                    Time.deltaTime * rotationSpeed);
        } else {
            character.SetIsWalking(false);
        }
    }

    private bool jumpStarted;
    private float jumpStartTime;
    private Vector3 jumpStartPosition;

    private void UpdateLinkJump() {
        if (jumpStartTime + jumpDuration > Time.time) {
            var jumpTime = Time.time - jumpStartTime;
            var jumpProgress = jumpTime / jumpDuration;
            if (jumpProgress < 0.2f) {
                jumpProgress = 0;
            } else {
                jumpProgress = 1 - ((1 - jumpProgress) / 0.8f); // remapping progress from 0.2 - 1.0 to 0.0 - 1.0
            }
            
            var jumpStart = jumpStartPosition;
            var jumpHightDelta = jumpCurve.Evaluate(jumpProgress) * jumpMaxHight * Vector3.up;
            var jumpDistanceVector = navMeshAgent.currentOffMeshLinkData.endPos - jumpStart;
            var jumpWidthDelta = jumpDistanceVector * jumpProgress;

            var desiredPosition = jumpStart + jumpHightDelta + jumpWidthDelta;
            transform.position = desiredPosition;
        } else {
            navMeshAgent.transform.position = navMeshAgent.currentOffMeshLinkData.endPos;
            navMeshAgent.CompleteOffMeshLink();
            jumpStarted = false;
        }
    }

    public void StopNavigation() {
        navMeshAgent.ExtResetDestination();
    }
}
