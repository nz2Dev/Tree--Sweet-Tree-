using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiencePlayer : MonoBehaviour {

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float cycleDelaySec = 30;
    [SerializeField] private float initVolume = 0.03f;

    private bool isActive = true;
    private float controledVolume;

    public bool IsActive => isActive;

    private void Awake() {
        SetVolume(initVolume);
    }

    public void BeginAmbience() {
        StopAllCoroutines();
        StartCoroutine(SequenceRoutine());
    }

    public void SetVolume(float volume) {
        controledVolume = volume;
        UpdateVolume();
    }

    private float transitionDuration;
    private SequenceState transitionSequence;
    private float startVolume;
    private float targetVolume;

    public void SetVolumeTransitionDuration(float duration) {
        transitionDuration = duration;
    }

    public void SetVolumeTransition(float volume) {
        transitionSequence = TweenUtils.StartSequence(transitionDuration);
        startVolume = controledVolume;
        targetVolume = volume;
    }

    private void UpdateVolumeTransition() {
        if (TweenUtils.TryUpdateSequence(transitionSequence, out var progress)) {
            SetVolume(Mathf.Lerp(startVolume, targetVolume, progress));
        }
        TweenUtils.TryFinishSequence(ref transitionSequence);
    }

    public void SetIsActive(bool isActive) {
        this.isActive = isActive;
        UpdateVolume();
    }

    private void UpdateVolume() {
        audioSource.volume = isActive ? controledVolume : 0;
    }

    private IEnumerator SequenceRoutine() {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length / audioSource.pitch);
        audioSource.Stop();
        yield return new WaitForSeconds(cycleDelaySec);
        BeginAmbience();
    }

    private void Update() {
        UpdateVolumeTransition();

        if (Application.isEditor && Input.GetKeyDown(KeyCode.R)) {
            BeginAmbience();
        }
    }
    
}
