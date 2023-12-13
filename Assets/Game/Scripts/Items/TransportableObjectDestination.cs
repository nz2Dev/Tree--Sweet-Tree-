using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransportableObjectDestination : MonoBehaviour {

    [SerializeField] private bool initIsIncluded = true;
    [SerializeField] private UnityEvent OnObjectPlaced;
    [SerializeField] private UnityEvent OnObjectRemoved;
    [SerializeField] private float layOutRadius = -1;
    [SerializeField] private GameObject layOutRadiusGraphics;

    private bool includeInActivation;

    public event Action<bool> OnContainObjectIsChanged;

    public bool IsExcludedFromActivation => !includeInActivation;
    public bool IsRestrictedByRadius => layOutRadius >= 0;
    public float LayOutRadius => layOutRadius;

    private void Awake() {
        SetIsIncluded(initIsIncluded);
    }

    public void CheckRadius(Transform checkTransform) {
        if (layOutRadiusGraphics != null && IsRestrictedByRadius) {
            var distance = Vector3.Distance(transform.GetPositionXZ(), checkTransform.GetPositionXZ());
            layOutRadiusGraphics.SetActive(distance > LayOutRadius);
        }
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

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (IsRestrictedByRadius) {
            Gizmos.DrawWireSphere(transform.position, layOutRadius);
        }
    }
#endif

}
