using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    
    private bool working;

    public bool IsWorking {
        get => working;
        set {
            if (!working) {
                Open();
            }
            working = true;
        }
    }

    public event Action OnOpenRequest;

    public bool Put(PickUpable pickUpable) {
        if (working) {
            // var inventoryItem = pickUpable.GetInvetoryItem();
            pickUpable.gameObject.SetActive(false);
            return true;
        }
        return false;
    }

    public void Open() {
        OnOpenRequest?.Invoke();
    }

}
