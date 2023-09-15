using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionStateVisualsActivator : MonoBehaviour {

    [SerializeField] private SelectableObject selectable;
    [SerializeField] private GameObject defaultStateVisuals;
    [SerializeField] private GameObject selectedStateVisuals;

    private void Awake() {
        selectable.OnSelectionChanged += SelectableOnSelectionChanged;
    }

    private void Start() {
        selectedStateVisuals.SetActive(false);
    }

    private void SelectableOnSelectionChanged(bool selected) {
        // defaultStateVisuals.SetActive(!selected);
        selectedStateVisuals.SetActive(selected);
    }
    
}
