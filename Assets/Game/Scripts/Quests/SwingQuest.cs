using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SwingQuest : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private float cameraCutDuration = 0.9f;

    private bool activated;

    private void Awake() {
        vcam.m_Priority = 9;
    }

    public void Activate() {
        vcam.m_Priority += 2;
        activated = true;
        StartHideCharacter();
    }

    private void Update() {
        if (activated) {
            if (Time.time > startHideCharacterTime + cameraCutDuration) {
                OnChangeCharacterVisibility(false);
            }
        }
    }

    private float startHideCharacterTime;

    private void StartHideCharacter() {
        startHideCharacterTime = Time.time;   
    }

    private void OnChangeCharacterVisibility(bool visibility) {
        Player.LatestInstance.GetComponentInChildren<HovanetsCharacter>(true).gameObject.SetActive(visibility);
    }
    
}
