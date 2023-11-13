using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransportableObjectDestination : MonoBehaviour {

    private bool exludedFromActivation;

    public event Action<bool> OnContainObjectIsChanged;

    public bool IsExcludedFromActivation => exludedFromActivation;

    public void SetContainObject(bool containObject) {
        OnContainObjectIsChanged?.Invoke(containObject);
    }

    public void SetIsExcluded(bool excluded) {
        exludedFromActivation = excluded;
    }

}
