using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomNPC : MonoBehaviour {

    private Animator characterAnimator;

    private void Awake() {
        characterAnimator = GetComponentInChildren<Animator>();
    }

    public void Touch() {
        characterAnimator.SetTrigger("Active");
    }
}
