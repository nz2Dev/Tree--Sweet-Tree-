using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionStateOutlineActivator : MonoBehaviour {

    [SerializeField] private SelectableObject selectable;

    private Outline outline;

    private void Awake() {
        outline = GetComponent<Outline>();
        selectable.OnSelectionChanged += SelectableOnSelectionChanged;
    }

    private void Start() {
        outline.enabled = false;
    }

    private void SelectableOnSelectionChanged(bool selected) {
        outline.enabled = selected;
    }
}
