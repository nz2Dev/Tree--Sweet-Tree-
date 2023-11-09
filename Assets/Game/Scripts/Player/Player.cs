using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class Player : MonoBehaviour {
    
    [SerializeField] private Suggestion inventorySuggestion;
    [SerializeField] private AnimationCurve pickUpCurve;
    [SerializeField] private float pickingUpDuration = 0.4f;
    [SerializeField] private float jumpMaxHight = 1f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private float jumpStartDelay = 0.25f;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float rotationSpeed = 5;

    private NavMeshAgent navMeshAgent;
    private PopUpNotifications notifications;
    private HovanetsCharacter character;
    private Inventory inventory;

    private JumpPlatform platformUnder;
    private static Player latestInstance;

    public static Player LatestInstance => latestInstance;
    public JumpPlatform PlatformUnder => platformUnder;

    private void Awake() {
        latestInstance = this;

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
        UpdateGrabing();
        UpdateLayingOut();
        UpdateNavigation();
        UpdateMovePlatform();
        UpdateJump();
    }

    public void ReceiveNotification(Suggestion suggestion) {
        notifications.SendNotification(suggestion);
    }

    private TransportableObject grabbingObject;
    private SequenceState grabbingSequenceState;
    private TransformCapture grabbingStartTransformCapture;
    private Transform grabbingDestination;

    private TransportableObject grabbedObject;
    public TransportableObject GrabbedObject => grabbedObject;

    public void ActivateGrab(TransportableObject transportable) {
        grabbingSequenceState = TweenUtils.StartSequence(0.8f, 0.3f);
        grabbingStartTransformCapture = TweenUtils.CaptureTransforms(transportable.transform);
        grabbingDestination = Instantiate(transportable.Offsets, transform, false);
        grabbingObject = transportable;
    }

    private void UpdateGrabing() {
        if (TweenUtils.TryUpdateSequence(grabbingSequenceState, out var progress)) {
            TweenUtils.TweenAll(grabbingObject.transform, grabbingStartTransformCapture, grabbingDestination, progress);
        }
        if (TweenUtils.TryFinishSequence(ref grabbingSequenceState)) {
            grabbingObject.transform.SetParent(grabbingDestination, true);
            grabbingObject.OnGrabbed();
            grabbedObject = grabbingObject;
        }
    }

    private TransportableObject layingOutObject;
    private SequenceState layingOutSequenceState;
    private TransformCapture layingOutStartTransformCapture;
    private Transform layingOutDestination;

    public void ActivateLayOut(Transform targetPlace) {
        layingOutSequenceState = TweenUtils.StartSequence(0.8f, 0.3f);
        layingOutStartTransformCapture = TweenUtils.CaptureTransforms(grabbingObject.transform);
        layingOutDestination = targetPlace;
        layingOutObject = grabbingObject;

        var layingOutParent = layingOutObject.transform.parent;
        layingOutObject.transform.SetParent(null, true);
        Destroy(layingOutParent.gameObject);
    }

    private void UpdateLayingOut() {
        if (TweenUtils.TryUpdateSequence(layingOutSequenceState, out var progress)) {
            TweenUtils.TweenAll(layingOutObject.transform, layingOutStartTransformCapture, layingOutDestination, progress);
        }
        if (TweenUtils.TryFinishSequence(ref layingOutSequenceState)) {
            layingOutObject.OnLayedOut();
            grabbedObject = null;
        }
    }

    private PickUpable activePickUpable;
    private Transform pickUpDestination;
    private Vector3 activePickUpableStartPosition;
    private float pickUpActivationStartTime;
    private bool handleAutomatically;

    public bool CanPickUpFromHere(PickUpable pickUpable) {
        return Vector3.Distance(transform.position, pickUpable.transform.position) < pickUpable.PickUpRadius;
    }

    public void CancelPickUp() {
        // todo probably should call release on pickupable
        activePickUpable = null;
    }

    public void ActivatePickUp(PickUpable pickUpable, bool handleAutomatically = false) {
        this.handleAutomatically = handleAutomatically;
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

                if (handleAutomatically) {
                    HandlePickedUp();
                }
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
        if (platformUnder != null) {
            platformUnder.SetPlayerOnTop(false);
        }
        platformUnder = null;
        // cancel all the rest
        CancelPickUp();        

        navMeshAgent.destination = point;
        navMeshAgent.nextPosition = transform.position;
        navMeshAgent.updatePosition = true;
    }

    public float GetRemainingNavigationDistance() {
        return navMeshAgent.ExtGetRemainingDistanceAnyState();
    }

    public void UpdateNavigation() {
        if (navMeshAgent.isOnOffMeshLink && !linkJumpStarted) {
            linkJumpStarted = true;
            linkJumpStartTime = Time.time;   
            linkJumpStartPosition = transform.position;
            character.PlayJump();
        }

        if (linkJumpStarted) {
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

    private bool linkJumpStarted;
    private float linkJumpStartTime;
    private Vector3 linkJumpStartPosition;

    private void UpdateLinkJump() {
        if (linkJumpStartTime + jumpDuration > Time.time) {
            var jumpTime = Time.time - linkJumpStartTime;
            var jumpProgress = jumpTime / jumpDuration;
            if (jumpProgress < 0.2f) {
                jumpProgress = 0;
            } else {
                jumpProgress = 1 - ((1 - jumpProgress) / 0.8f); // remapping progress from 0.2 - 1.0 to 0.0 - 1.0
            }
            
            var jumpStart = linkJumpStartPosition;
            var jumpHightDelta = jumpCurve.Evaluate(jumpProgress) * jumpMaxHight * Vector3.up;
            var jumpDistanceVector = navMeshAgent.currentOffMeshLinkData.endPos - jumpStart;
            var jumpWidthDelta = jumpDistanceVector * jumpProgress;

            var desiredPosition = jumpStart + jumpHightDelta + jumpWidthDelta;
            transform.position = desiredPosition;
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                    Quaternion.LookRotation(Vector3.ProjectOnPlane(jumpDistanceVector, Vector3.up), Vector3.up), 
                    Time.deltaTime * rotationSpeed);
        } else {
            navMeshAgent.transform.position = navMeshAgent.currentOffMeshLinkData.endPos;
            navMeshAgent.CompleteOffMeshLink();
            linkJumpStarted = false;
        }
    }

    public void StopNavigation() {
        navMeshAgent.ExtResetDestination();
    }

    private Vector3 jumpStartPosition;
    private Vector3 jumpEndPosition;
    private SequenceState jumpSequenceState;

    public void ActivateJump(JumpPlatform target) {
        if (target != null && target.IsActive) {
            jumpSequenceState = TweenUtils.StartSequence(jumpDuration, jumpStartDelay);
            jumpStartPosition = transform.position;
            jumpEndPosition = target.jumpEndPoint.position;
            character.PlayJump();
            navMeshAgent.updatePosition = false;
            platformUnder = target;
        } else {
            character.PlayJump();
        }
    }

    private void UpdateJump() {
        if (TweenUtils.TryUpdateSequence(jumpSequenceState, out var jumpProgress)) {
            var jumpStart = jumpStartPosition;
            var jumpHightDelta = jumpCurve.Evaluate(jumpProgress) * jumpMaxHight * Vector3.up;
            var jumpDistanceVector = jumpEndPosition - jumpStart;
            var jumpWidthDelta = jumpDistanceVector * jumpProgress;

            var desiredPosition = jumpStart + jumpHightDelta + jumpWidthDelta;
            transform.position = desiredPosition;
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                    Quaternion.LookRotation(Vector3.ProjectOnPlane(jumpDistanceVector, Vector3.up), Vector3.up), 
                    Time.deltaTime * rotationSpeed);
        }
        if (TweenUtils.TryFinishSequence(ref jumpSequenceState)) {
            platformUnder.SetPlayerOnTop(true);
        }
    }

    public bool IsInJump() {
        return jumpSequenceState.active;
    }

    private MovePlatform activeMovePlatform;

    public void ActivateMovePlatform(MovePlatform movePlatform) {
        activeMovePlatform = movePlatform;
        navMeshAgent.updatePosition = false;
        transform.position = movePlatform.Start.position;
    }

    private void UpdateMovePlatform() {
        if (activeMovePlatform != null) {
            var movementSpeed = navMeshAgent.speed * 0.5f;

            var platformVector = activeMovePlatform.End.position - activeMovePlatform.Start.position;
            var moveDelta = platformVector.normalized * movementSpeed * Time.deltaTime;;

            var endVector = activeMovePlatform.End.position - transform.position;
            var isApproachingEnd = endVector.magnitude > moveDelta.magnitude * 3f;

            if (isApproachingEnd) {
                transform.position += moveDelta;
                character.SetIsWalking(true);
                character.SetWalkMotionSpeed(movementSpeed);
            } else {
                character.SetIsWalking(false);
                activeMovePlatform = null;
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, 
                        Quaternion.LookRotation(Vector3.ProjectOnPlane(platformVector, Vector3.up), Vector3.up), 
                        Time.deltaTime * rotationSpeed);
        }
    }

    public bool IsPlatforming() {
        return activeMovePlatform != null;
    }

    public void CancelMovePlatform() {
        activeMovePlatform = null;
        character.SetIsWalking(false);
    }
}
