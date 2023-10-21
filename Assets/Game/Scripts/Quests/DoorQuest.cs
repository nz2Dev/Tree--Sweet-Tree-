using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DoorQuest : MonoBehaviour {
    
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private DoorStates door;

    private void Awake() {
        vcam.m_Priority = 9;
    }

    public void Activate() {
        vcam.m_Priority += 2;
        door.SetState(DoorStates.State.Quest);
    }

}
