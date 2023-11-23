using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class TorchQuest : MonoBehaviour {

    [SerializeField] private SwingStates swingStates;
    [SerializeField] private ObjectSelector objectSelector;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private float cameraCutDuration = 0.9f;
    [SerializeField] private Transform itemHubTransform;
    [SerializeField] private GameObject applyZone;
    [SerializeField] private Sprite cupElementIcon;
    [SerializeField] private Transform cupElementDestination;
    [SerializeField] private Sprite candleElementIcon;
    [SerializeField] private Transform candleElementDestination;
    [SerializeField] private Sprite matchElementIcon;
    [SerializeField] private AnimationCurve shakingCurve;
    [SerializeField] private float shakingCurveScale = 0.1f;
    [SerializeField] private float shakingDuration = 0.5f;
    [SerializeField] private int selectionIgnoreLayer;
    [SerializeField] private float applyingDuration = 0.8f;
    [SerializeField] private AnimationCurve applyingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private GameObject torch;

    private bool activated;
    private List<GameObject> appliedObjectsList;

    private void Awake() {
        vcam.m_Priority = 9;
        appliedObjectsList = new List<GameObject>();
    }

    private void Start() {
        applyZone.SetActive(false);
        torch.SetActive(false);
    }

    public void Activate() {
        vcam.m_Priority += 2;
        activated = true;

        StartChangeCharacterVisibility(false);
        BindToInventoryEvents();
        swingStates.SetState(SwingStates.State.Quest);
    }

    public void Deactivate() {
        vcam.m_Priority -= 2;
        activated = false;

        StartChangeCharacterVisibility(true);
        UnbindFromInventoryEvents();

        if (torch.activeSelf) {
            swingStates.SetState(SwingStates.State.Stationar);
        } else {
            swingStates.SetState(SwingStates.State.Activator);
        }
    }

    private void BindToInventoryEvents() {
        Player.LatestInstance.GetComponent<Inventory>().OnItemActivated += LaidOutNewItem;
    }

    private void UnbindFromInventoryEvents() {
        Player.LatestInstance.GetComponent<Inventory>().OnItemActivated -= LaidOutNewItem;
    }

    private GameObject laidOutObject;
    private Item laidOutInventoryItem;
    private Sprite laidOutObjectIcon; // using inventory item sprite specification to identify elements

    private void LaidOutNewItem(int itemIndex) {
        if (laidOutObjectIcon != null) {
            return;
        }

        var item = Player.LatestInstance.GetComponent<Inventory>().PullItem(itemIndex);
        laidOutInventoryItem = item;
        laidOutObject = GameObject.Instantiate(item.prefab, Vector3.zero, Quaternion.identity);
        laidOutObject.transform.SetParent(itemHubTransform, false);
        laidOutObjectIcon = item.icon;
    }

    private void HighlightLaidOut() {
        laidOutObject.GetComponent<SelectableObject>().Highlight();
    }

    private void StopHighlightLaidOut() {
        laidOutObject.GetComponent<SelectableObject>().StopHighlighting();
    }

    private void ReturnLaidOutToInventory() {
        var pickupable = laidOutObject.AddComponent<PickUpable>();
        pickupable.Setup(laidOutInventoryItem);
        Player.LatestInstance.ActivatePickUp(pickupable, handleAutomatically: true);
    }

    private void ForgetLaidOutItem() {
        laidOutObject = null;
        laidOutObjectIcon = null;
    }

    private float startChangeCharacterVisibilityTime;
    private bool changingCharacterVisibility;
    private bool visibilityFlag;

    private void StartChangeCharacterVisibility(bool visibilityFlag) {
        this.visibilityFlag = visibilityFlag;
        startChangeCharacterVisibilityTime = Time.time;   
        changingCharacterVisibility = true;
    }

    private void UpdateHideCharacter() {
        if (changingCharacterVisibility && Time.time > startChangeCharacterVisibilityTime + cameraCutDuration) {
            Player.LatestInstance.GetComponentInChildren<HovanetsCharacter>(true).gameObject.SetActive(visibilityFlag);
            changingCharacterVisibility = false;
        }
    }

    private GameObject applyingObject;
    private Sprite applyingObjectIcon;
    private TweenState applyingTweenState;

    private void StartApplyingObject(GameObject gameObject, Sprite gameObjectSprite, Transform destination) {
        applyingObject = gameObject;
        applyingObjectIcon = gameObjectSprite;
        applyingTweenState = TweenUtils.StartTween(applyingObject.transform, destination, applyingDuration, applyingCurve);
    }

    private void UpdateApplyingAnimation() {
        if (TweenUtils.TryFinishTween(ref applyingTweenState)) {
            OnApplyingFinished();
        }
    }

    private void OnApplyingFinished() {
        applyingObject.GetComponent<SelectableObject>().StopHighlighting();
        applyingObject.GetComponent<SelectableObject>().OverrideCollidingLayer(selectionIgnoreLayer);
        appliedObjectsList.Add(applyingObject);
        
        if (applyingObjectIcon == matchElementIcon) {
            foreach (var appliedObject in appliedObjectsList) {
                Destroy(appliedObject);
            }

            torch.SetActive(true);
        }
    }

    private DeltaTweenState shakingTweenState;

    private void StartShakingObject(GameObject gameObject) {
        shakingTweenState = TweenUtils.StartDeltaTween(
            target: gameObject.transform, 
            direction: itemHubTransform.right, 
            scale: shakingCurveScale, 
            duration: shakingDuration, 
            curve: shakingCurve);
    }

    private void UpdateShakingObject() {
        TweenUtils.TryFinishDeltaTween(ref shakingTweenState);
    }

    private bool IsLaidOutIsSelected() {
        return objectSelector.Selected != null && objectSelector.Selected.gameObject == laidOutObject;
    }

    private bool IsApplyZoneIsSelected() {
        return objectSelector.Selected != null && objectSelector.Selected.gameObject == applyZone;
    }

    private bool IsTorchIsSelected() {
        return objectSelector.Selected != null && objectSelector.Selected.gameObject == torch;
    }

    private bool IsLaidOutCanBeApplied() {
        if (laidOutObjectIcon == cupElementIcon) {
            return appliedObjectsList.Count == 0;
        }
        if (laidOutObjectIcon == candleElementIcon) {
            return appliedObjectsList.Count == 1;
        }
        if (laidOutObjectIcon == matchElementIcon) {
            return appliedObjectsList.Count == 2;
        }
        return false;
    }

    private Transform GetLaidOutObjectDestinationTransform() {
        if (laidOutObjectIcon == cupElementIcon) {
            return cupElementDestination;
        }
        if (laidOutObjectIcon == candleElementIcon) {
            return candleElementDestination;
        }
        return applyZone.transform;
    }

    private void HandleQuestInput() {
        if (activated) {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                HandleClick();
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Deactivate();
            }
        }
    }

    private void HandleClick() {
        if (torch.activeSelf) {
            HandleDeactivation();
        }

        if (!isElementChosen) {
            HandleChosing();
        } else {
            HandleApplying();
        }
    }

    private void HandleDeactivation() {
        if (IsTorchIsSelected()) {
            torch.GetComponent<SelectableObject>().OverrideCollidingLayer(selectionIgnoreLayer);
            Deactivate();
        }
    }

    private bool isElementChosen;

    private void HandleChosing() {
        if (IsLaidOutIsSelected()) {
            HighlightLaidOut();
            applyZone.SetActive(true);
            isElementChosen = true;
        }
    }

    private void HandleApplying() {
        if (IsApplyZoneIsSelected()) {
            applyZone.SetActive(false);
            isElementChosen = false;

            if (IsLaidOutCanBeApplied()) {
                StartApplyingObject(laidOutObject, laidOutObjectIcon, GetLaidOutObjectDestinationTransform());
                StopHighlightLaidOut();
                ForgetLaidOutItem();
            } else {
                StartShakingObject(laidOutObject);
                ReturnLaidOutToInventory();
                ForgetLaidOutItem();
            }
        } else {
            StartShakingObject(laidOutObject);
        }
    }

    private void Update() {
        UpdateHideCharacter();

        HandleQuestInput();

        UpdateApplyingAnimation();

        UpdateShakingObject();
    }

}
