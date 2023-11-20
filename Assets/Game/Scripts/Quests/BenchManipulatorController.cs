using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BenchManipulatorController : MonoBehaviour {

    [SerializeField] private BenchStates benchStates;
    [SerializeField] private BenchManipulator benchManipulator;
    [SerializeField] private CinemachineVirtualCamera manipulatorVCam;
    [SerializeField] private ObjectSelector objectSelector;
    [SerializeField] private Color snappedColor = Color.blue;

    private bool activated;
    private bool manipulating;
    private bool approving;
    private Vector3 raycastPosition;

    private void Awake() {
        benchStates.Activator.OnActivated += OnActivated;
    }

    private void Start() {
        benchManipulator.Stop();
    }

    private void OnActivated() {
        activated = true;
        benchStates.SetState(BenchStates.State.Starter);
        manipulatorVCam.m_Priority += 2;
    }

    private void OnDeactivated() {
        activated = false;
        manipulatorVCam.m_Priority -= 2;
    }

    private void Update() {
        if (activated) {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                if (objectSelector.Selected != null && objectSelector.Selected == benchStates.Starter) {
                    benchStates.SetState(BenchStates.State.Manipulatable);
                    benchManipulator.Begin(benchStates.Manipulated);
                    manipulating = true;
                }
            }
        }

        if (manipulating) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100, benchManipulator.ManipulationSurface)) {
                raycastPosition = hit.point;
            }

            bool isPositionSnapped = benchManipulator.TryMoveToSnap(raycastPosition);

            if (isPositionSnapped) {
                if (benchManipulator.TryRotateToSnap(Input.mouseScrollDelta.y)) {
                    benchManipulator.Stop();
                    benchStates.SetManipulatedSnappedColor(snappedColor);
                    manipulating = false;
                    approving = true;
                }
            }
        }

        if (approving) {
            if (Input.GetMouseButtonDown(0)) {
                approving = false;
                benchStates.SetState(BenchStates.State.Stationar);
                OnDeactivated();
            }
        }
    }

}
