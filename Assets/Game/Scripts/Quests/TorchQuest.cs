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

    private bool activated;
    private List<Sprite> appliedItemsList;

    private void Awake() {
        vcam.m_Priority = 9;
        appliedItemsList = new List<Sprite>();
    }

    private void Start() {
        applyZone.SetActive(false);
    }

    public void Activate() {
        vcam.m_Priority += 2;
        activated = true;

        StartHideCharacter();
        BindToInventoryEvents();
        swingStates.SetState(SwingStates.State.Quest);
    }

    private void BindToInventoryEvents() {
        Player.LatestInstance.GetComponent<Inventory>().OnItemActivated += LaidOutNewItem;
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

    private float startHideCharacterTime;
    private bool hidingCharacter;

    private void StartHideCharacter() {
        startHideCharacterTime = Time.time;   
        hidingCharacter = true;
    }

    private void UpdateHideCharacter() {
        if (hidingCharacter && Time.time > startHideCharacterTime + cameraCutDuration) {
            Player.LatestInstance.GetComponentInChildren<HovanetsCharacter>(true).gameObject.SetActive(false);
            hidingCharacter = false;
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
        appliedItemsList.Add(applyingObjectIcon);
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
            return appliedItemsList.Count == 0;
        }
        if (laidOutObjectIcon == candleElementIcon) {
            return appliedItemsList.Count == 1;
        }
        if (laidOutObjectIcon == matchElementIcon) {
            return appliedItemsList.Count == 2;
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
