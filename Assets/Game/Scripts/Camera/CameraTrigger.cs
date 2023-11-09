using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private Collider target;

    private void OnTriggerEnter(Collider other) {
        if (other == target) {
            vcam.m_Priority += 2;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other == target) {
            vcam.m_Priority -= 2;
        }
    }
    
}
