using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomNPC : MonoBehaviour {

    [SerializeField] private Suggestion thirstySuggestion;

    private Animator characterAnimator;

    private void Awake() {
        characterAnimator = GetComponentInChildren<Animator>();
    }

    public bool TryTouch(out Suggestion response) {
        characterAnimator.SetTrigger("Active");
        response = thirstySuggestion;
        return true;
    }
}
