using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransportableObjectDestination : MonoBehaviour {

    [SerializeField] private bool initIsIncluded = true;
    [SerializeField] private UnityEvent OnObjectPlaced;
    [SerializeField] private UnityEvent OnObjectRemoved;

    private bool includeInActivation;

    public event Action<bool> OnContainObjectIsChanged;

    public bool IsExcludedFromActivation => !includeInActivation;

    private void Awake() {
        SetIsIncluded(initIsIncluded);
    }

    public void SetContainObject(bool containObject) {
        OnContainObjectIsChanged?.Invoke(containObject);
        if (containObject) {
            OnObjectPlaced?.Invoke();
        } else {
            OnObjectRemoved?.Invoke();
        }
    }

    public void SetIsIncluded(bool included) {
        includeInActivation = included;
    }

}
