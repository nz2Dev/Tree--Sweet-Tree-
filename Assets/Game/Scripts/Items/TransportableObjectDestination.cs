using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransportableObjectDestination : MonoBehaviour {

    [SerializeField] private bool initIsExcluded;
    [SerializeField] private UnityEvent<bool> OnContainObjectIsChangedEvent;

    private bool exludedFromActivation;

    public event Action<bool> OnContainObjectIsChanged;

    public bool IsExcludedFromActivation => exludedFromActivation;

    private void Awake() {
        SetIsExcluded(initIsExcluded);
    }

    public void SetContainObject(bool containObject) {
        OnContainObjectIsChanged?.Invoke(containObject);
        OnContainObjectIsChangedEvent?.Invoke(containObject);
    }

    public void SetIsExcluded(bool excluded) {
        exludedFromActivation = excluded;
    }

}
