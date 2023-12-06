using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSetter : MonoBehaviour {
    [SerializeField] private Transform sourceTransform;
    [SerializeField] private Transform targetTransform;

    
    public void SetSourcePosition() {
        targetTransform.position = sourceTransform.position;
        targetTransform.rotation = sourceTransform.rotation;
        Debug.Log("Set " + targetTransform.name + " position to " + sourceTransform.position);
        targetTransform.GetComponent<Player>().AdjustPositionOnNavMesh();
    }
    
}
