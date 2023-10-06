using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationObject : MonoBehaviour {
    [SerializeField] private float activationRadius = 2f;

    public float ActivationRadius => activationRadius;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, activationRadius);        
    }
#endif

}
