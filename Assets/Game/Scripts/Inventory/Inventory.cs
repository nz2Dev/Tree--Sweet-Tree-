using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Item {
    public Sprite icon;
}

public class Inventory : MonoBehaviour {
    
    [SerializeField] private bool initialIsWorkingState;
    [SerializeField] private Item[] initialItems;

    private bool working;
    private List<Item> items;

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
        items = new List<Item>();
        working = initialIsWorkingState;
    }

    private void Start() {
        items.AddRange(initialItems);
    }

    public bool Put(Item item) {
        if (working) {
            items.Add(item);
            OnItemAdded?.Invoke(items.Count - 1);
            return true;
        }
        return false;
    }

    public void ActivateItem(int index) {
        if (index < items.Count) {
            items.RemoveAt(index);
            OnItemRemoved?.Invoke(index);
        }
    }

    public Item GetItem(int index) {
        return items[index];
    }

    public void Open() {
        OnOpenRequest?.Invoke();
    }

}
