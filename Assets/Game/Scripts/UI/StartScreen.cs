using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartScreen : MonoBehaviour {
    
    [SerializeField] private GameObject root;
    [SerializeField] private bool autoCompleteInEditor = false;
    [SerializeField] private UnityEvent OnPlayActivated;

    private void Start() {
        root.SetActive(true);
        if (Application.isEditor && autoCompleteInEditor) {
            OnPlay();
        }
    }

    public void OnPlay() {
        root.SetActive(false);
        OnPlayActivated?.Invoke();
    }

}
