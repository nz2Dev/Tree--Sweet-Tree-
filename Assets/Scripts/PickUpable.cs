using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpable : MonoBehaviour {
    
    [SerializeField] private float pickUpRadius = 1f;

    public float PickUpRadius => pickUpRadius;

    public void Release() {
        if (Physics.Raycast(transform.position, -Vector3.up, out var hitInfo, 10)) {
            transform.position = hitInfo.point;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);        
    }
#endif

}
