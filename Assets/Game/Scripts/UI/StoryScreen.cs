using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class StoryScreen : MonoBehaviour {
    
    [SerializeField] private GameObject root;
    [SerializeField] private float autoSkipDelaySec = 3f;
    [SerializeField] private bool enableAutoSkip = true;
    [SerializeField] private bool enableSkipInEditor = false;
    [SerializeField] private bool enableCutscene = true;
    [SerializeField] private PlayableDirector storyCutsceneDirector;

    private bool isCompleted;

    private void Awake() {
        root.SetActive(false);
    }

    public void Open() {
        isCompleted = false;
        root.SetActive(true);

        if (Application.isEditor && enableSkipInEditor) {
            OnSkip();
            return;
        }

        if (enableAutoSkip) {
            this.StartDelayedActionCallback(autoSkipDelaySec, () => {
                OnSkip();
            });
        }
    }

    public void OnSkip() {
        Complete();
    }

    private void Complete() {
        if (!isCompleted) {
            isCompleted = true;
            root.SetActive(false);

            if (enableCutscene) {
                storyCutsceneDirector.Play();
            }
        }
    }

}
