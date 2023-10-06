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
    }

}
