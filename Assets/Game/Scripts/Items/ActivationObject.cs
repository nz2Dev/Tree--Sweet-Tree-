using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivationObject : MonoBehaviour {
    [SerializeField] private float activationRadius = 2f;
    [SerializeField] private Transform activationPoint;
    [SerializeField] private UnityEvent OnActivatedEvent;

    public float ActivationRadius => activationRadius;
    public Transform ActivationPoint => activationPoint;

    public event Action OnActivated;

    private void Awake() {
        if (activationPoint == null) {
            activationPoint = transform;
        }
    }

    public void Activate() {
        OnActivated?.Invoke();
        
        if (OnActivatedEvent != null) {
            OnActivatedEvent.Invoke();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        var position = activationPoint == null ? transform.position : activationPoint.position;
        Gizmos.DrawWireSphere(position, activationRadius);        
    }
#endif

}
