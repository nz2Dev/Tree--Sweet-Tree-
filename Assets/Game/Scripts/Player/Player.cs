using System;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {
    
    [SerializeField] private Suggestion inventorySuggestion;
    [SerializeField] private Suggestion noSpaceSuggestion;
    [SerializeField] private AnimationCurve pickUpCurve;
    [SerializeField] private float pickingUpDuration = 0.4f;
    [SerializeField] private float jumpMaxHight = 1f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private float jumpStartDelay = 0.25f;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private float grabDuration = 0.5f;

    private NavMeshAgent navMeshAgent;
    private PopUpNotifications notifications;
    private HovanetsCharacter character;
    private Inventory inventory;

    private JumpPlatform platformUnder;
    private static Player latestInstance;

    public static Player LatestInstance => latestInstance;
    public JumpPlatform PlatformUnder => platformUnder;
    public Inventory Inventory => inventory;

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

    private Coroutine hideCoroutine;
    private Coroutine showCoroutine;

    public void HideCharacterDelayed(float delay) {
        StopCharacterHideShowCoroutines();
        hideCoroutine = this.StartDelayedActionCallback(delay, () => {
            character.gameObject.SetActive(false);
        });
    }

    public void ShowCharacterDelayed(float delay) {
        StopCharacterHideShowCoroutines();
        showCoroutine = this.StartDelayedActionCallback(delay, () => {
            character.gameObject.SetActive(true);
        });
    }

    private void StopCharacterHideShowCoroutines() {
        if (hideCoroutine != null) {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
        if (showCoroutine != null) {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }
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
        if (grabbedObject != null) {
            character.PlayJump();
            return;
        }

        grabbingSequenceState = TweenUtils.StartSequence(grabDuration, 0.3f);
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
            grabbingObject.Grabed();
            grabbedObject = grabbingObject;
        }
    }

    private TransportableObject layingOutObject;
    private SequenceState layingOutSequenceState;
    private TransformCapture layingOutStartTransformCapture;
    private GameObject layingOutDestination;

    public void ActivateLayOut(GameObject layOutDestination) {
        layingOutSequenceState = TweenUtils.StartSequence(grabDuration, 0.3f);
        layingOutStartTransformCapture = TweenUtils.CaptureTransforms(grabbingObject.transform);
        layingOutDestination = layOutDestination;
        layingOutObject = grabbingObject;

        var layingOutParent = layingOutObject.transform.parent;
        layingOutObject.transform.SetParent(null, true);
        Destroy(layingOutParent.gameObject);
    }

    private void UpdateLayingOut() {
        if (TweenUtils.TryUpdateSequence(layingOutSequenceState, out var progress)) {
            TweenUtils.TweenAll(layingOutObject.transform, layingOutStartTransformCapture, layingOutDestination.transform, progress);
        }
        if (TweenUtils.TryFinishSequence(ref layingOutSequenceState)) {
            layingOutObject.LayOut(layingOutDestination);
            grabbedObject = null;
        }
    }

    private PickUpable activePickUpable;
    private Transform pickUpDestination;
    private Vector3 activePickUpableStartPosition;
    private float pickUpActivationStartTime;

    public bool CanPickUpFromHere(PickUpable pickUpable) {
        return Vector3.Distance(transform.position, pickUpable.transform.position) < pickUpable.PickUpRadius;
    }

    public void ActivatePickUp(PickUpable pickUpable) {
        if (activePickUpable != null) {
            HandlePickedUp();
        }

        navMeshAgent.ExtResetDestination();

        activePickUpable = pickUpable;
        activePickUpableStartPosition = pickUpable.transform.position;
        pickUpActivationStartTime = Time.time;

        if (pickUpable.TryGetComponent<SelectableObject>(out var selectable)) {
            selectable.Highlight();
        }
        
        if (pickUpable.name == "Bag" || !inventory.IsWorking) {
            pickUpDestination = character.HandsLocation;
        } else {
            pickUpDestination = character.BagLocation;
        }
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
                HandlePickedUp();
            }
        }
    }

    private void HandlePickedUp() {
        var pickedUp = activePickUpable;
        if (pickedUp.TryGetComponent<SelectableObject>(out var selectable)) {
            selectable.Highlight();
        }
        activePickUpable = null;
        
        if (pickedUp.name == "Bag") {
            inventory.IsWorking = true;
            character.SetBagEquiped(inventory.IsWorking);
            // destory object from hands
            pickedUp.DestroySelf(consumed: true);
            return;
        }

        if (!inventory.IsWorking) {
            // drop object from hands
            pickedUp.transform.position = activePickUpableStartPosition;

            notifications.SendNotification(inventorySuggestion);
            return;
        } 
        
        if (!inventory.HasSpace()) {
            pickedUp.transform.position = activePickUpableStartPosition;

            notifications.SendNotification(noSpaceSuggestion);
            return;
        } 
        
        // handle object by inventory
        inventory.Put(pickedUp.InventoryItemSO);
        pickedUp.DestroySelf(consumed: true);
    }

    public void DropInventoryItem(int itemIndex) {
        if (inventory.GetItem(itemIndex).DropPrefab != null) {
            var pulledItem = inventory.PullItem(itemIndex);
            var dropObject = Instantiate(pulledItem.DropPrefab, Vector3.zero, Quaternion.identity);
            dropObject.transform.position = transform.position + transform.forward * 0.5f;
        }
    }

    public void AdjustPositionOnNavMesh() {
        navMeshAgent.Warp(transform.position);
    }

    public void ActivateNavigation(Vector3 point) {
        if (platformUnder != null) {
            platformUnder.SetPlayerOnTop(false);
        }
        platformUnder = null;
        // cancel all the rest
        // CancelPickUp();        

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
            if (NavMesh.SamplePosition(transform.position, out var navMeshHit, 0.5f, NavMesh.AllAreas)) {
                navMeshAgent.Warp(navMeshHit.position);
            }
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

    public void ActivateEmotion() {
        character.PlayEmotion();
    }
}
