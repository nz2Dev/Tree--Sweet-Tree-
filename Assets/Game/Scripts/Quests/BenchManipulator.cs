using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BenchManipulator : MonoBehaviour {

    [SerializeField] private BenchStates benchStates;
    [SerializeField] private CinemachineVirtualCamera manipulatorVCam;
    [SerializeField] private ObjectSelector objectSelector;
    [SerializeField] private Color snappedColor = Color.blue;
    [SerializeField] private GameObject benchTransformReference;
    [SerializeField] private LayerMask manipulationSurface;
    [SerializeField] private float snapSpeed = 10;
    [SerializeField] private float snapDistance = 0.3f;

    private bool activated;
    private bool manipulating;
    private bool approving;
    private Vector3 raycastPosition;

    private void Awake() {
        benchStates.Activator.OnActivated += ActivationObjectOnActivated;
    }

    private void Start() {
        benchTransformReference.SetActive(false);
    }

    private void ActivationObjectOnActivated() {
        activated = true;
        benchStates.SetState(BenchStates.State.Starter);
        manipulatorVCam.m_Priority++;
        manipulatorVCam.m_Priority++;
    }

    private void ManipulationActivatorOnActivated() {
        benchStates.SetState(BenchStates.State.Manipulatable);
        benchTransformReference.SetActive(true);
        manipulating = true;
    }

    private void ManipulationFinished() {
        benchTransformReference.SetActive(false);
        benchStates.SetManipulatedSnappedColor(snappedColor);
        manipulating = false;
        approving = true;
    }

    private void ManipulationApproved() {
        activated = false;
        approving = false;
        benchStates.SetState(BenchStates.State.Stationar);

        manipulatorVCam.m_Priority--;
        manipulatorVCam.m_Priority--;
    }

    private void Update() {
        if (activated) {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                if (objectSelector.Selected != null && objectSelector.Selected == benchStates.Starter) {
                    ManipulationActivatorOnActivated();
                }
            }
        }

        if (manipulating) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100, manipulationSurface)) {
                raycastPosition = hit.point;
            }

            var snapped = false;
            if (Vector3.Distance(raycastPosition, benchTransformReference.transform.position) < snapDistance) {
                snapped = true;
                raycastPosition = benchTransformReference.transform.position;
            } 

            var manipulatedBench = benchStates.Manipulated;
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
