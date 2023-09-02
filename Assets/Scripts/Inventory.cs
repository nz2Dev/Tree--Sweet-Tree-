using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    
    private bool working;

    public bool IsWorking => working;

    public bool Put(PickUpable pickUpable) {
        if (working) {
            // var inventoryItem = pickUpable.GetInvetoryItem();
            pickUpable.gameObject.SetActive(false);
            return true;
        }
        return false;
    }

}
