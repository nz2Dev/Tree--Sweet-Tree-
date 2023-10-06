using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BenchManipulator : MonoBehaviour {

    [SerializeField] private ActivationObject activator;
    [SerializeField] private CinemachineVirtualCamera manipulatorVCam;
    [SerializeField] private ActivationObject manipulationActivator;
    [SerializeField] private GameObject manipulatedBench;
    [SerializeField] private LayerMask manipulationSurface;
    [SerializeField] private float snapSpeed = 10;

    private bool manipulating;
    private Vector3 raycastPosition;

    private void Awake() {
        activator.OnActivated += ActivationObjectOnActivated;
        manipulationActivator.OnActivated += ManipulationActivatorOnActivated;
    }

    private void ActivationObjectOnActivated() {
        manipulatorVCam.m_Priority++;
        activator.gameObject.SetActive(false);
        manipulationActivator.gameObject.SetActive(true);
    }

    private void ManipulationActivatorOnActivated() {
        manipulationActivator.gameObject.SetActive(false);
        manipulatedBench.gameObject.SetActive(true);
        manipulating = true;
    }

    private void Update() {
        if (manipulating) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100, manipulationSurface)) {
                raycastPosition = hit.point;
            }

            manipulatedBench.transform.position = Vector3.Lerp(manipulatedBench.transform.position, raycastPosition, Time.deltaTime * snapSpeed);
        }
    }

}
