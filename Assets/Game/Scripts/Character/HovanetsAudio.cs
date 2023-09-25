using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovanetsAudio : MonoBehaviour {
    
    [SerializeField] private AudioSource leftFootSource;
    [SerializeField] private AudioSource rightFootSource;

    [SerializeField] private AudioClip[] walkClips;

    public void PlayLeftFoot() {
        leftFootSource.clip = SelectRandom(walkClips);
        leftFootSource.Play();
    }

    public void PlayRightFoot() {
        rightFootSource.clip = SelectRandom(walkClips);
        rightFootSource.Play();
    }

    private AudioClip SelectRandom(AudioClip[] clips) {
        return clips[Random.Range(0, clips.Length)];
    }

}
