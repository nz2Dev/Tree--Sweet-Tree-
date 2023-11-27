using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BenchManipulator : MonoBehaviour {

    [SerializeField] private GameObject benchTransformReference;
    [SerializeField] private LayerMask manipulationSurface;
    [SerializeField] private float snapSpeed = 10;
    [SerializeField] private float snapDistance = 0.3f;

    private Bench manipulated;
    private float rotationProgress;

    public bool IsManipulating => manipulated != null;
    public LayerMask ManipulationSurface => manipulationSurface;

    private void Start() {
        benchTransformReference.SetActive(false);
    }

    public void Begin(Bench manipulated) {
        this.manipulated = manipulated;
        benchTransformReference.SetActive(true);
        rotationProgress = manipulated.GetBaseRotationInDegrees(); 
    }

    public void Stop() {
        benchTransformReference.SetActive(false);
    }

    public Plane GetMovePlane() {
        return new Plane(Vector3.up, benchTransformReference.transform.position);
    }

    public bool TryMoveToSnap(in Vector3 targetPosition) {
        var snapped = false;
        var movePosition = targetPosition;
        
        var distanceToRef = Vector3.Distance(movePosition, benchTransformReference.transform.position);
        if (distanceToRef < snapDistance) {
            movePosition = benchTransformReference.transform.position;
            snapped = true;
        } else if (distanceToRef > 2) {
            var refToTarget = targetPosition - benchTransformReference.transform.position;
            movePosition = benchTransformReference.transform.position + refToTarget.normalized * 2f;
        }
        
        benchTransformReference.SetActive(!snapped);
        if (!snapped) {
            manipulated.transform.position = Vector3.Lerp(manipulated.transform.position, movePosition, Time.deltaTime * snapSpeed);
        } else {
            manipulated.transform.position = movePosition;
        }
        return snapped;
    }

    public void AlignBase() {
        manipulated.transform.localRotation = Quaternion.Lerp(
            manipulated.transform.localRotation, 
            Quaternion.LookRotation(-Vector3.forward, Vector3.up), 
            Time.deltaTime);
    }

    public bool TryRotateToSnap(in float rotationInput) {
        rotationProgress += rotationInput;
        rotationProgress = Mathf.Clamp(rotationProgress, -90, 0);

        var snapped = false;
        if (rotationProgress < 10 && rotationProgress > -10) {
            snapped = true;
            manipulated.SetBaseRotation(0);
        } else {
            manipulated.SetBaseRotation(rotationProgress);
        }

        return snapped;
    }
}
