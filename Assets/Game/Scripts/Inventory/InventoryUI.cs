using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject baseRoot;
    [SerializeField] private Sprite emptySlotSprite;
    [SerializeField] private RectTransform baseMask;
    [SerializeField] private RectTransform baseArea;
    [SerializeField] private GameObject itemsRoot;
    [SerializeField] private RectTransform itemsMask;
    [SerializeField] private RectTransform itemsContainer;
    [SerializeField] private float changeDuration = 0.5f;
    [SerializeField] private RectTransform activator;
    [SerializeField] private float activatorChangeDuration = 0.25f;
    [SerializeField] private RectTransform highlightingItem;
    [SerializeField] private AnimationCurve highlightMoveCurve;
    [SerializeField] private float moveMultiplier = 1f;
    [SerializeField] private float highlightDelay = 0.5f;
    [SerializeField] private float highlightDuration = 0.5f;

    private Vector2 highlightingItemStartPosition;

    private bool changingContainer;
    private bool changeContainerStateIsOpen;
    private float changeContainerStartTime;
    private Vector2 changeContainerBaseMaskToSizeDelta;
    private Vector2 changeContainerBaseMaskFromSizeDelta;
    private Vector2 changeContainerItemsMaskFromSizeDelta;
    private Vector2 changeContainerItemsMaskToSizeDelta;

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
        inventory.OnItemRemoved += (index) => ChangeItemsDisplayState();
    }

    private void Start() {
        highlightingItemStartPosition = highlightingItem.anchoredPosition;
        highlightingItem.gameObject.SetActive(false);

        ChangeItemsDisplayState();
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
        baseRoot.SetActive(false);
        itemsRoot.SetActive(false);
        activator.gameObject.SetActive(false);
    }    

    public void HighlightItem(int index) {
        ChangeItemsDisplayState();
        if (!baseRoot.activeSelf) {
            OpenDirectly();
        }
        PlayHighlightOnSlot(index);
    }

    public void OnItemClicked(int index) {
        inventory.ActivateItem(index);
    }

    private void PlayHighlightOnSlot(int index) {
        playingHighlight = true;
        playingHighlightStartTime = Time.time + highlightDelay;
        playingHighlightSlotIndex = index;

        var highlightSlot = itemsContainer.GetChild(index);
        highlightSlot.GetComponent<CanvasGroup>().alpha = 0;
        BindSlotToItem(highlightingItem, inventory.GetItem(index));
    }

    private void ChangeContainerState(bool open) {
        changingContainer = true;
        changeContainerStateIsOpen = open;
        changeContainerStartTime = Time.time;
        changeContainerBaseMaskFromSizeDelta = open ? new Vector2(-70, baseArea.sizeDelta.y) : baseMask.sizeDelta;
        changeContainerBaseMaskToSizeDelta = open ? baseArea.sizeDelta : new Vector2(-70, baseArea.sizeDelta.y);
        changeContainerItemsMaskFromSizeDelta = open ? new Vector2(0, itemsContainer.sizeDelta.y) : itemsContainer.sizeDelta;
        changeContainerItemsMaskToSizeDelta = open ? itemsContainer.sizeDelta : new Vector2(0, itemsContainer.sizeDelta.y);
        baseRoot.SetActive(true);
        itemsRoot.SetActive(true);
    }

    private void ChangeItemsDisplayState() {
        for (int i = 0; i < itemsContainer.childCount; i++) {
            var itemSlot = itemsContainer.GetChild(i);
            if (i >= inventory.ItemsCount) {
                itemSlot.gameObject.SetActive(false);
            } else {
                itemSlot.gameObject.SetActive(true);
                BindSlotToItem(itemSlot, inventory.GetItem(i));
            }
        }
    }

    private void BindSlotToItem(Transform itemSlotTransform, ItemSO item) {
        var itemSlotImage = itemSlotTransform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
        itemSlotImage.sprite = item.Icon;
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
                baseMask.sizeDelta = Vector2.Lerp(changeContainerBaseMaskFromSizeDelta, changeContainerBaseMaskToSizeDelta, changeProgress);
                itemsMask.sizeDelta = Vector2.Lerp(changeContainerItemsMaskFromSizeDelta, changeContainerItemsMaskToSizeDelta, changeProgress);
            } else {
                changingContainer = false;
                baseMask.sizeDelta = changeContainerBaseMaskToSizeDelta;
                itemsMask.sizeDelta = changeContainerItemsMaskToSizeDelta;

                if (!changeContainerStateIsOpen) {
                    baseRoot.SetActive(false);
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
                highlightingItem.gameObject.SetActive(true);

                if (Time.time < highlightEndTime) {
                    var highlightProgress = (Time.time - playingHighlightStartTime) / highlightDuration;

                    var highlightSlot = itemsContainer.GetChild(playingHighlightSlotIndex) as RectTransform;
                    var positionDelta = highlightSlot.anchoredPosition - highlightingItemStartPosition;

                    var hightExtra = highlightMoveCurve.Evaluate(highlightProgress) * moveMultiplier;
                    highlightingItem.anchoredPosition = highlightingItemStartPosition + positionDelta * highlightProgress + Vector2.up * hightExtra;
                } else {
                    playingHighlight = false;

                    highlightingItem.gameObject.SetActive(false);
                    var highlightSlot = itemsContainer.GetChild(playingHighlightSlotIndex) as RectTransform;
                    highlightSlot.GetComponent<CanvasGroup>().alpha = 1;
                }
            }
        }
    }

}
