using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ClipsSet {
    public AudioClip[] clips;
}

public class HovanetsAudio : MonoBehaviour {
    
    [SerializeField] private AudioSource leftFootSource;
    [SerializeField] private AudioSource rightFootSource;

    [SerializeField] private ClipsSet[] walkClipSets;
    [SerializeField] private int selectedSetIndex;

    public void PlayLeftFoot() {
        leftFootSource.clip = SelectRandom(walkClipSets[selectedSetIndex].clips);
        leftFootSource.Play();
    }

    public void PlayRightFoot() {
        rightFootSource.clip = SelectRandom(walkClipSets[selectedSetIndex].clips);
        rightFootSource.Play();
    }

    private AudioClip SelectRandom(AudioClip[] clips) {
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }

}
