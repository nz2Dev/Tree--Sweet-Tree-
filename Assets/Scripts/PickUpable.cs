using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpable : MonoBehaviour {
    
    [SerializeField] private float pickUpRadius = 1f;

    public float PickUpRadius => pickUpRadius;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);        
    }
#endif

}
