using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DistanceDebug : MonoBehaviour {
     [SerializeField] private GameObject target;

     private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target.transform.position);
        var distance = Vector3.Distance(target.transform.position, transform.position);
        Handles.Label(transform.position, "Distance: " + distance);
     }
    
}
