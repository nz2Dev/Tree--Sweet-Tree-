using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectableObjectEvents : MonoBehaviour {
    [SerializeField] private UnityEvent OnClicked;

    private SelectableObject selectableObject;

    private void OnEnable() {
        selectableObject.OnClicked += SelectableObjectOnClicked;
    }

    private void SelectableObjectOnClicked() {
        OnClicked?.Invoke();
    }

    private void OnDisable() {
        selectableObject.OnClicked -= SelectableObjectOnClicked;
    }

}
