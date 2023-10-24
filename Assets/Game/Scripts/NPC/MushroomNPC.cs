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

    public void OnActivated(Player player) {
        characterAnimator.SetTrigger("Active");
        player.ReceiveNotification(thirstySuggestion);
    }

}
