using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour {

    [SerializeField] private Texture2D selectionCursorTexture;

    public Texture2D SelectionCursorTexture => selectionCursorTexture;

    public event Action<bool> OnSelectionChanged;

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

    private void NotifyChanges() {
        OnSelectionChanged?.Invoke(isSelected);
    }

}
