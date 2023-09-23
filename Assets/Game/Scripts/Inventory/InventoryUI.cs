using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject inventoryRoot;
    [SerializeField] private Sprite emptySlotSprite;
    [SerializeField] private RectTransform mask;
    [SerializeField] private RectTransform container;
    [SerializeField] private float changeDuration = 0.5f;
    [SerializeField] private RectTransform activator;
    [SerializeField] private float activatorChangeDuration = 0.25f;
    [SerializeField] private AnimationCurve highlightScaleCurve;
    [SerializeField] private float scaleMultiplier = 1f;
    [SerializeField] private AnimationCurve highlightMoveCurve;
    [SerializeField] private float moveMultiplier = 1f;
    [SerializeField] private float highlightDelay = 0.5f;
    [SerializeField] private float highlightDuration = 0.5f;

    private bool changingContainer;
    private bool changeContainerStateIsOpen;
    private float changeContainerStartTime;
    private Vector2 changeContainerToSizeDelta;
    private Vector2 changeContainerFromSizeDelta;

    private bool changingActivator;
    private bool changeActivatorStateIsVisible;
    private float changeActivatorStartTime;
    private float changeActivatorFromScale;
    private float changeActivatorToScale;

    private bool playingHighlight;
    private int playingHighlightSlotIndex;
    private float playingHighlightStartTime;

    private void Awake() {
        inventory.OnOpenRequest += OpenDirectly;
        inventory.OnItemAdded += HighlightItem;
    }

    private void Start() {
        ChangeItemsSprite();
        if (inventory.IsWorking) {
            Close();
        } else {
            Disable();
        }
    }

    public void Open() {
        ChangeActivatorState(visible: false);
    }

    public void OpenDirectly() {
        activator.gameObject.SetActive(false);
        ChangeContainerState(true);
    }

    public void Close() {
        ChangeContainerState(open: false);
    }

    public void Disable() {
        inventoryRoot.SetActive(false);
        activator.gameObject.SetActive(false);
    }    

    public void HighlightItem(int index) {
        ChangeItemsSprite();
        if (!inventoryRoot.activeSelf) {
            OpenDirectly();
        }
        PlayHighlightOnSlot(index);
    }

    private void PlayHighlightOnSlot(int index) {
        playingHighlight = true;
        playingHighlightStartTime = Time.time + highlightDelay;
        playingHighlightSlotIndex = index;
    }

    private void ChangeContainerState(bool open) {
        changingContainer = true;
        changeContainerStateIsOpen = open;
        changeContainerStartTime = Time.time;
        changeContainerFromSizeDelta = mask.sizeDelta;
        changeContainerToSizeDelta = open ? container.sizeDelta : new Vector2(0, container.sizeDelta.y);
        inventoryRoot.SetActive(true);
    }

    private void ChangeItemsSprite() {
        for (int i = 0; i < container.childCount; i++) {
            var itemSlot = container.GetChild(i);
            var itemSlotImage = itemSlot.GetChild(1).GetComponent<UnityEngine.UI.Image>();
            if (i >= inventory.ItemsCount) {
                itemSlotImage.enabled = false;
            } else {
                var item = inventory.GetItem(i);
                itemSlotImage.enabled = true;
                itemSlotImage.sprite = item.icon;
            }
        }
    }

    private void ChangeActivatorState(bool visible) {
        changingActivator = true;
        changeActivatorStateIsVisible = visible;
        changeActivatorStartTime = Time.time;
        changeActivatorFromScale = visible ? 0.5f : 1f;
        changeActivatorToScale = visible ? 1 : 0;
        activator.gameObject.SetActive(true);
    }

    private void Update() {
        if (changingContainer) {
            var changeEndTime = changeContainerStartTime + changeDuration;
            if (Time.time < changeEndTime) {
                var changeProgress = (Time.time - changeContainerStartTime) / changeDuration;
                mask.sizeDelta = Vector2.Lerp(changeContainerFromSizeDelta, changeContainerToSizeDelta, changeProgress);
            } else {
                changingContainer = false;
                mask.sizeDelta = changeContainerToSizeDelta;

                if (!changeContainerStateIsOpen) {
                    inventoryRoot.SetActive(false);
                    ChangeActivatorState(visible: true);
                }
            }
        }

        if (changingActivator) {
            var changeEndTime = changeActivatorStartTime + activatorChangeDuration;
            if (Time.time < changeEndTime) {
                var changeProgress = (Time.time - changeActivatorStartTime) / activatorChangeDuration;
                var scale = Mathf.Lerp(changeActivatorFromScale, changeActivatorToScale, changeProgress);
                activator.localScale = new Vector3(scale, scale, scale);
            } else {
                changingActivator = false;
                activator.localScale = new Vector3(changeActivatorToScale, changeActivatorToScale, changeActivatorToScale);

                if (!changeActivatorStateIsVisible) {
                    // activator.gameObject.setActive(false); well, we are scaling, so no needs to additionaly enable/disable GO
                    ChangeContainerState(open: true);
                }
            }
        }

        if (playingHighlight) {
            var highlightEndTime = playingHighlightStartTime + highlightDuration;
            if (playingHighlightStartTime < Time.time) {
                mask.GetComponent<Image>().enabled = false;
                if (Time.time < highlightEndTime) {
                    var highlightProgress = (Time.time - playingHighlightStartTime) / highlightDuration;

                    var yPos = highlightMoveCurve.Evaluate(highlightProgress) * moveMultiplier;
                    var slotIconTransform = container.GetChild(playingHighlightSlotIndex).GetChild(1).transform as RectTransform;
                    slotIconTransform.anchoredPosition = new Vector2(0, yPos);

                    var scale = 1 + highlightScaleCurve.Evaluate(highlightProgress) * scaleMultiplier;
                    var slotBackgroundTransform = container.GetChild(playingHighlightSlotIndex).GetChild(0).transform;
                    slotBackgroundTransform.localScale = new Vector3(scale, scale, scale);
                } else {
                    mask.GetComponent<Image>().enabled = true;
                    playingHighlight = false;
                    var slotIconTransform = container.GetChild(playingHighlightSlotIndex).GetChild(1).transform as RectTransform;
                    slotIconTransform.anchoredPosition = Vector2.zero;
                    var slotBackgroundTransform = container.GetChild(playingHighlightSlotIndex).GetChild(0).transform;
                    slotBackgroundTransform.localScale = Vector3.one;
                }
            }
        }
    }

}
