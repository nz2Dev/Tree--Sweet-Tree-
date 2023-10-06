using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BenchManipulator : MonoBehaviour {

    [SerializeField] private ActivationObject activator;
    [SerializeField] private CinemachineVirtualCamera manipulatorVCam;
    [SerializeField] private GameObject manipulatedBench;

    private void Awake() {
        activator.OnActivated += ActivationObjectOnActivated;
    }

    private void ActivationObjectOnActivated() {
        manipulatorVCam.m_Priority++;
        manipulatedBench.SetActive(true);
        activator.gameObject.SetActive(false);
    }

}
