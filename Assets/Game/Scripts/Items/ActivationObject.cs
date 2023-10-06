using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationObject : MonoBehaviour {
    [SerializeField] private float activationRadius = 2f;

    public float ActivationRadius => activationRadius;

    public event Action OnActivated;

    public void Activate() {
        OnActivated?.Invoke();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, activationRadius);        
    }
#endif

}
