using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BenchManipulator : MonoBehaviour {

    [SerializeField] private ActivationObject activator;
    [SerializeField] private GameObject actualBench;
    [SerializeField] private CinemachineVirtualCamera manipulatorVCam;
    [SerializeField] private ActivationObject manipulationActivator;
    [SerializeField] private GameObject manipulatedBench;
    [SerializeField] private Color snappedColor = Color.blue;
    [SerializeField] private GameObject benchTransformReference;
    [SerializeField] private LayerMask manipulationSurface;
    [SerializeField] private float snapSpeed = 10;
    [SerializeField] private float snapDistance = 0.3f;

    private bool manipulating;
    private bool approving;
    private Vector3 raycastPosition;

    public bool InFocus => manipulating || approving;

    private void Awake() {
        activator.OnActivated += ActivationObjectOnActivated;
        manipulationActivator.OnActivated += ManipulationActivatorOnActivated;
    }

    private void Start() {
        manipulatedBench.SetActive(false);
        manipulationActivator.gameObject.SetActive(false);
        benchTransformReference.SetActive(false);
        actualBench.SetActive(false);
    }

    private void ActivationObjectOnActivated() {
        activator.gameObject.SetActive(false);
        manipulationActivator.gameObject.SetActive(true);
        manipulatorVCam.m_Priority++;
        manipulatorVCam.m_Priority++;
    }

    private void ManipulationActivatorOnActivated() {
        manipulationActivator.gameObject.SetActive(false);
        manipulatedBench.gameObject.SetActive(true);
        benchTransformReference.SetActive(true);
        manipulating = true;
    }

    private void ManipulationFinished() {
        benchTransformReference.SetActive(false);
        manipulatedBench.GetComponentInChildren<Renderer>().material.color = snappedColor;
        manipulating = false;
        approving = true;
    }

    private void ManipulationApproved() {
        approving = false;
        actualBench.SetActive(true);
        manipulatedBench.SetActive(false);
        manipulatorVCam.m_Priority--;
        manipulatorVCam.m_Priority--;
    }

    public void UpdateControl() {
        if (manipulating) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100, manipulationSurface)) {
                raycastPosition = hit.point;
            }

            var snapped = false;
            if (Vector3.Distance(raycastPosition, benchTransformReference.transform.position) < snapDistance) {
                snapped = true;
                raycastPosition = benchTransformReference.transform.position;
            } 

            manipulatedBench.transform.position = Vector3.Lerp(manipulatedBench.transform.position, raycastPosition, Time.deltaTime * snapSpeed);

            if (snapped) {
                var newRotation = manipulatedBench.transform.rotation * Quaternion.AngleAxis(Input.mouseScrollDelta.y, Vector3.right);
                var referenceUp = benchTransformReference.transform.TransformDirection(Vector3.up);
                var manipulatedUp = newRotation * Vector3.up;
                var dotProduct = Vector3.Dot(referenceUp, manipulatedUp);
                if (dotProduct > 0.9995f) {
                    manipulating = false;
                    manipulatedBench.transform.rotation = benchTransformReference.transform.rotation;
                    ManipulationFinished();
                } else if (dotProduct > 0.1f) {
                    manipulatedBench.transform.rotation = newRotation;
                }
            }
        }

        if (approving) {
            if (Input.GetMouseButtonDown(0)) {
                ManipulationApproved();
            }
        }
    }

}
