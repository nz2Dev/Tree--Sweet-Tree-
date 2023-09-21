using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {
    
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject inventoryRoot;
    [SerializeField] private RectTransform mask;
    [SerializeField] private RectTransform container;
    [SerializeField] private float changeDuration = 0.5f;

    private bool changing;
    private float changeStartTime;
    private Vector2 changeSizeDelta;
    private Vector2 lastSizeDelta;

    private void Awake() {
        inventory.OnOpenRequest += Open;
        inventoryRoot.SetActive(true);
    }

    private void Start() {
        Close();
    }

    public void Open() {
        ChangeState(true);
    }

    private void ChangeState(bool open) {
        changing = true;
        changeStartTime = Time.time;
        lastSizeDelta = mask.sizeDelta;
        changeSizeDelta = open ? container.sizeDelta : new Vector2(0, container.sizeDelta.y);
    }

    private void Update() {
        if (changing) {
            var changeEndTime = changeStartTime + changeDuration;
            if (Time.time < changeEndTime) {
                var changeProgress = (Time.time - changeStartTime) / changeDuration;
                mask.sizeDelta = Vector2.Lerp(lastSizeDelta, changeSizeDelta, changeProgress);
            } else {
                changing = false;
                mask.sizeDelta = changeSizeDelta;
            }
        }
    }

    public void Close() {
        ChangeState(false);
    }

}
