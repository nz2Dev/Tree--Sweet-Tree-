using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchManipulator : MonoBehaviour {

    [SerializeField] private GameObject benchTransformReference;
    [SerializeField] private LayerMask manipulationSurface;
    [SerializeField] private float snapSpeed = 10;
    [SerializeField] private float snapDistance = 0.3f;

    private GameObject manipulated;

    public bool IsManipulating => manipulated != null;
    public LayerMask ManipulationSurface => manipulationSurface;

    private void Start() {
        benchTransformReference.SetActive(false);
    }

    public void Begin(GameObject manipulated) {
        this.manipulated = manipulated;
        benchTransformReference.SetActive(true);
    }

    public void Stop() {
        benchTransformReference.SetActive(false);
    }

    public bool TryMoveToSnap(in Vector3 targetPosition) {
        var snapped = false;
        var movePosition = targetPosition;
        
        if (Vector3.Distance(movePosition, benchTransformReference.transform.position) < snapDistance) {
            movePosition = benchTransformReference.transform.position;
            snapped = true;
        } 
        
        manipulated.transform.position = Vector3.Lerp(manipulated.transform.position, movePosition, Time.deltaTime * snapSpeed);
        return snapped;
    }

    public bool TryRotateToSnap(in float rightAxisRotationDelta) {
        var newRotation = manipulated.transform.rotation * Quaternion.AngleAxis(rightAxisRotationDelta, Vector3.right);
        var referenceUp = benchTransformReference.transform.TransformDirection(Vector3.up);
        var manipulatedUp = newRotation * Vector3.up;
        
        var snapped = false;
        var dotProduct = Vector3.Dot(referenceUp, manipulatedUp);
        if (dotProduct > 0.9995f) {
            snapped = true;
            manipulated.transform.rotation = benchTransformReference.transform.rotation;
        } else if (dotProduct > 0.1f) {
            manipulated.transform.rotation = newRotation;
        }

        return snapped;
    }
}
