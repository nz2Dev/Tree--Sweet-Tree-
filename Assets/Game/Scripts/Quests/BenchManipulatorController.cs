using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BenchManipulatorController : MonoBehaviour {

    [SerializeField] private BenchManipulator benchManipulator;
    [SerializeField] private CinemachineVirtualCamera manipulatorVCam;
    [SerializeField] private ObjectSelector objectSelector;

    private BenchStates bench;

    private bool activated;
    private bool manipulating;
    private bool approving;
    private Vector3 raycastPosition;

    private void Start() {
        benchManipulator.Stop();
    }

    public void Activate(BenchStates bench) {
        this.bench = bench;
        activated = true;
        OnActivated();
    }

    private void OnActivated() {
        manipulatorVCam.m_Priority += 2;
    }

    public void Deactivate() {
        activated = false;
        OnDeactivated();
    }

    private void OnDeactivated() {
        manipulatorVCam.m_Priority -= 2;
    }

    private void Update() {
        if (activated) {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                if (objectSelector.Selected != null && objectSelector.Selected == bench.Selectable) {
                    bench.SetManipulatableMoveable();
                    benchManipulator.Begin(bench.gameObject);
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
                    bench.SetManipulatableSnapped();
                    manipulating = false;
                    approving = true;
                }
            }
        }

        if (approving) {
            if (Input.GetMouseButtonDown(0)) {
                approving = false;
                bench.SetManipulatableStationar();
                OnDeactivated();
            }
        }
    }

}
