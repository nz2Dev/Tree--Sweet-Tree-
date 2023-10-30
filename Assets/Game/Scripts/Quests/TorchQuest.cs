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
    [SerializeField] private AnimationCurve shakingCurve;
    [SerializeField] private float shakingCurveScale = 0.1f;
    [SerializeField] private float shakingDuration = 0.5f;

    private bool activated;
    private GameObject laidOutObject;
    private Sprite laidOutObjectIcon; // using inventory item sprite specification to identify elements
    private bool applyRegime;
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
        Player.LatestInstance.GetComponent<Inventory>().OnItemActivated += InventoryOnItemActivated;
    }

    private void InventoryOnItemActivated(Item item) {
        laidOutObject = GameObject.Instantiate(item.prefab, Vector3.zero, Quaternion.identity);
        laidOutObject.transform.SetParent(itemHubTransform, false);
        laidOutObjectIcon = item.icon;
    }

    private float startHideCharacterTime;

    private void StartHideCharacter() {
        startHideCharacterTime = Time.time;   
    }

    private void OnChangeCharacterVisibility(bool visibility) {
        Player.LatestInstance.GetComponentInChildren<HovanetsCharacter>(true).gameObject.SetActive(visibility);
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

    private void Update() {
        if (activated) {
            if (Time.time > startHideCharacterTime + cameraCutDuration) {
                OnChangeCharacterVisibility(false);
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                if (!applyRegime) {
                    if (objectSelector.Selected != null && objectSelector.Selected.gameObject == laidOutObject) {
                        objectSelector.Selected.Highlight();
                        applyZone.SetActive(true);
                        applyRegime = true;
                    }
                } else if (applyRegime) {
                    bool validToApply = false;

                    if (objectSelector.Selected != null && objectSelector.Selected.gameObject == applyZone) {
                        if (laidOutObjectIcon == cupElementIcon && appliedItemsList.Count == 0) {
                            validToApply = true;        
                        }
                        if (laidOutObjectIcon == candleElementIcon && appliedItemsList.Count == 1 && appliedItemsList[0] == cupElementIcon) {
                            validToApply = true;
                        }
                    }

                    applyZone.SetActive(false);
                    applyRegime = false;

                    if (validToApply) {
                        StartApplyingObject(laidOutObject, laidOutObjectIcon);
                        laidOutObject = null;
                        laidOutObjectIcon = null;
                    } else {
                        StartShakingObject(laidOutObject);
                        laidOutObject.GetComponent<SelectableObject>().StopHighlighting();
                    }
                }
            }
        }

        if (applyingAnimation) {
            var applyingDuration = 1.0f;
            var endTime = startApplyingTime + applyingDuration;
            if (Time.time < endTime) {
                var progress = (Time.time - startApplyingTime) / applyingDuration;
                applyingObject.transform.position = Vector3.Lerp(itemHubTransform.position, applyZone.transform.position, progress);
            } else {
                applyingAnimation = false;
                applyingObject.transform.position = applyZone.transform.position;

                applyingObject.GetComponent<SelectableObject>().StopHighlighting();
                appliedItemsList.Add(applyingObjectIcon);
            }
        }

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

}
