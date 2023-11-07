using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour {

    [SerializeField] private bool initIsDetectable = true;

    public event Action<bool> OnSelectionChanged;
    public event Action<bool> OnHighlightChanged;
    public event Action OnClicked;

    private bool isSelected;
    private bool isHighlighted;

    private int nativeColliderLayer;

    public bool IsSelected => isSelected;
    public bool IsHighlighted => isHighlighted;

    private void Awake() {
        var collider = GetComponentInChildren<Collider>();
        if (collider == null) {
            Debug.LogError("no physic collider in SelectaleObject children");
        }
        nativeColliderLayer = collider.gameObject.layer;
        SetIsDetectable(initIsDetectable);
    }

    public void OverrideCollidingLayer(int layer) {
        var collider = GetComponentInChildren<Collider>();
        collider.gameObject.layer = layer;
    }

    public void ResetCollidingLayer() {
        var collider = GetComponentInChildren<Collider>();
        collider.gameObject.layer = nativeColliderLayer;
    }

    public void SetIsDetectable(bool detectable) {
        var collider = GetComponentInChildren<Collider>();
        collider.enabled = detectable;
    }

    public void Click() {
        OnClicked?.Invoke();
    }

    public void OnSelected() {
        isSelected = true;
        NotifyOnSelectionChanged();
    }

    public void OnUnselected() {
        isSelected = false;
        NotifyOnSelectionChanged();
    }

    private void NotifyOnSelectionChanged() {
        OnSelectionChanged?.Invoke(isSelected);
    }

    public void Highlight() {
        isHighlighted = true;
        OnHighlightChanged?.Invoke(true);
    }

    public void StopHighlighting() {
        isHighlighted = false;
        OnHighlightChanged?.Invoke(false);
    }

}
