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
    [SerializeField] private Sprite candleElementIcon;
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
        swingStates.SetState(SwingStates.State.Stationar);
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

    private void LaidOutNewItem(Item item) {
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
    private float startApplyingTime;
    private bool applyingAnimation;

    private void StartApplyingObject(GameObject gameObject, Sprite gameObjectSprite) {
        applyingObject = gameObject;
        applyingObjectIcon = gameObjectSprite;
        applyingAnimation = true;
        startApplyingTime = Time.time;
    }

    private void UpdateApplyingAnimation() {
        if (applyingAnimation) {
            var endTime = startApplyingTime + applyingDuration;
            if (Time.time < endTime) {
                var progress = applyingCurve.Evaluate((Time.time - startApplyingTime) / applyingDuration);
                applyingObject.transform.position = Vector3.Lerp(itemHubTransform.position, applyZone.transform.position, progress);
            } else {
                applyingAnimation = false;
                applyingObject.transform.position = applyZone.transform.position;

                OnApplyingFinished();
            }
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
            Deactivate();
        }
    }

    private GameObject shakingObject;
    private Vector3 shakingObjectStartPosition;
    private float startShakingTime;
    private bool shakingAnimation;

    private void StartShakingObject(GameObject gameObject) {
        shakingAnimation = true;
        startShakingTime = Time.time;
        shakingObject = gameObject;
        shakingObjectStartPosition = gameObject.transform.position;
    }

    private void UpdateShakingObject() {
        if (shakingAnimation) {
            var endTime = startShakingTime + shakingDuration;
            if (Time.time < endTime) {
                var progress = (Time.time - startShakingTime) / shakingDuration;
                var shakingValue = shakingCurve.Evaluate(progress) * shakingCurveScale;
                var shakingDelta = itemHubTransform.right * shakingValue;
                shakingObject.transform.position = shakingObjectStartPosition + shakingDelta;
            } else {
                shakingAnimation = false;
                shakingObject.transform.position = shakingObjectStartPosition;
            }
        }
    }

    private bool IsLaidOutIsSelected() {
        return objectSelector.Selected != null && objectSelector.Selected.gameObject == laidOutObject;
    }

    private bool IsApplyZoneIsSelected() {
        return objectSelector.Selected != null && objectSelector.Selected.gameObject == applyZone;
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

    private void HandleQuestInput() {
        if (activated) {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                HandleClick();
            }
        }
    }

    private void HandleClick() {
        if (!isElementChosen) {
            HandleChosing();
        } else {
            HandleApplying();
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
                StartApplyingObject(laidOutObject, laidOutObjectIcon);
                StopHighlightLaidOut();
                ForgetLaidOutItem();
            } else {
                StartShakingObject(laidOutObject);
                ReturnLaidOutToInventory();
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
