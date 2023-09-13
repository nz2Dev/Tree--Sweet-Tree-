using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {
    
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject inventoryRoot;

    private void Awake() {
        inventory.OnOpenRequest += Open;
    }

    private void Start() {
        Close();
    }

    public void Open() {
        inventoryRoot.SetActive(true);
    }

    public void Close() {
        inventoryRoot.SetActive(false);
    }

}
