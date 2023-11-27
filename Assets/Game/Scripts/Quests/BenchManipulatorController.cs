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

    private Bench bench;

    private bool activated;
    private bool manipulating;
    private bool approving;
    private Vector3 raycastPosition;

    public void Activate(Bench bench) {
        this.bench = bench;
        StartCoroutine(NextFrame(() => {
            activated = true;
            OnActivated();
        }));
    }

    private IEnumerator NextFrame(Action action) {
        yield return new WaitForEndOfFrame();
        action?.Invoke();
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
                    benchManipulator.Begin(bench);
                    manipulating = true;
                }
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) {
                bench.SetManipulatableStationar(final: false);
                benchManipulator.Stop();
                manipulating = false;
                Deactivate();
            }
        }

        if (manipulating) {
            var movePlane = benchManipulator.GetMovePlane();
            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (movePlane.Raycast(cameraRay, out var enterDist)) {
                raycastPosition = cameraRay.GetPoint(enterDist);
            }

            benchManipulator.AlignBase();
            bool isPositionSnapped = benchManipulator.TryMoveToSnap(raycastPosition);
            bool isRotationSnapped = benchManipulator.TryRotateToSnap(-Input.mouseScrollDelta.y);

            if (isPositionSnapped && isRotationSnapped) {
                benchManipulator.Stop();
                bench.SetManipulatableSnapped();
                manipulating = false;
                approving = true;
            }
        }

        if (approving) {
            if (Input.GetMouseButtonDown(0)) {
                approving = false;
                bench.SetManipulatableStationar(final: true);
                OnDeactivated();
            }
        }
    }

}
