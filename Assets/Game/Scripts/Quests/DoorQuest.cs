using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DoorQuest : MonoBehaviour {
    
    [SerializeField] private CinemachineVirtualCamera vcam;

    private void Awake() {
        vcam.m_Priority = 9;
    }

    public void Activate() {
        vcam.m_Priority += 2;
    }

}
