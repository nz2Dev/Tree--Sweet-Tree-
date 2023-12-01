using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class Inventory : MonoBehaviour {
    
    [SerializeField] private bool initialIsWorkingState;
    [SerializeField] private ItemSO[] initialItems;

    private bool working;
    private List<ItemSO> items;
    private List<Action<int>> itemActivationControlersOrder;

    public int ItemsCount => items.Count;
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
    public event Action<int> OnItemAdded; // notifies at what index a new item was placed
    public event Action<int> OnItemRemoved;

    private void Awake() {
        items = new List<ItemSO>();
        itemActivationControlersOrder = new List<Action<int>>();
        working = initialIsWorkingState;
        items.AddRange(initialItems);
    }

    public void RegisterItemActivationController(Action<int> itemActivationCallback) {
        itemActivationControlersOrder.Add(itemActivationCallback);
    }

    public void UnregisterItemActivationController(Action<int> itemActivationCallback) {
        itemActivationControlersOrder.Remove(itemActivationCallback);
    }

    public bool HasSpace() {
        return items.Count < 5;
    }

    public bool Put(ItemSO item) {
        if (working) {
            items.Add(item);
            OnItemAdded?.Invoke(items.Count - 1);
            return true;
        }
        return false;
    }

    public void ActivateItem(int index) {
        if (index < items.Count) {
            var currentItemActivationCallback = itemActivationControlersOrder.LastOrDefault();
            currentItemActivationCallback?.Invoke(index);
        }
    }

    public ItemSO PullItem(int index) {
        var item = items[index];
        items.RemoveAt(index);
        OnItemRemoved?.Invoke(index);
        return item;
    }

    public ItemSO GetItem(int index) {
        return items[index];
    }

    public void Open() {
        OnOpenRequest?.Invoke();
    }

}
