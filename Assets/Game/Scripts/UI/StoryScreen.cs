using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryScreen : MonoBehaviour {
    
    [SerializeField] private GameObject root;
    [SerializeField] private float autoSkipDelaySec = 3f;
    [SerializeField] private bool enableAutoSkip = true;
    [SerializeField] private UnityEvent OnCompleted;

    private bool isCompleted;

    private void Awake() {
        root.SetActive(false);
    }

    public void Open() {
        isCompleted = false;
        root.SetActive(true);

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
            OnCompleted?.Invoke();
        }
    }

}
