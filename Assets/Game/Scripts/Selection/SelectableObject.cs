using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour {

    public event Action<bool> OnSelectionChanged;
    public event Action<bool> OnHighlightChanged;

    private bool isSelected;

    private void Awake() {
        var collider = GetComponentInChildren<Collider>();
        if (collider == null) {
            Debug.LogError("no physic collider in SelectaleObject children");
        }
    }

    public void MarkSelected() {
        isSelected = true;
        NotifyChanges();
    }

    public void MarkUnselected() {
        isSelected = false;
        NotifyChanges();
    }

    public void Highlight() {
        OnHighlightChanged?.Invoke(true);
    }

    public void StopHighlighting() {
        OnHighlightChanged?.Invoke(false);
    }

    private void NotifyChanges() {
        OnSelectionChanged?.Invoke(isSelected);
    }

}
