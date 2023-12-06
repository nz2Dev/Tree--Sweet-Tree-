using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiencePlayer : MonoBehaviour {

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float cycleDelaySec = 30;

    private bool isActive = true;
    private float cachedVolumeLevel;

    public bool IsActive => isActive;

    private void Awake() {
        cachedVolumeLevel = audioSource.volume;
    }

    public void BeginAmbience() {
        StopAllCoroutines();
        StartCoroutine(SequenceRoutine());
    }

    public void SetIsActive(bool isActive) {
        this.isActive = isActive;
        audioSource.volume = isActive ? cachedVolumeLevel : 0;
    }

    private IEnumerator SequenceRoutine() {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.time);
        audioSource.Stop();
        yield return new WaitForSeconds(cycleDelaySec);
        BeginAmbience();
    }

    private void Update() {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.R)) {
            BeginAmbience();
        }
    }
    
}
