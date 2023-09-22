using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {
    
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject inventoryRoot;
    [SerializeField] private RectTransform mask;
    [SerializeField] private RectTransform container;
    [SerializeField] private float changeDuration = 0.5f;
    [SerializeField] private RectTransform activator;
    [SerializeField] private float activatorChangeDuration = 0.25f;

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

    private void Awake() {
        inventory.OnOpenRequest += OpenDirectly;
        inventoryRoot.SetActive(true);
    }

    private void Start() {
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

    private void ChangeContainerState(bool open) {
        changingContainer = true;
        changeContainerStateIsOpen = open;
        changeContainerStartTime = Time.time;
        changeContainerFromSizeDelta = mask.sizeDelta;
        changeContainerToSizeDelta = open ? container.sizeDelta : new Vector2(0, container.sizeDelta.y);
        inventoryRoot.SetActive(true);
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
    }

}
