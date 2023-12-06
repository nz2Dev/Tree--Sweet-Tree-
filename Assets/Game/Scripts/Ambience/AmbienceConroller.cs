using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceConroller : MonoBehaviour {
    [SerializeField] private GameObject onText;
    [SerializeField] private GameObject offText;
    [SerializeField] private AmbiencePlayer ambiencePlayer;

    private void Start() {
        UpdateText();
    }

    private void UpdateText() {
        onText.SetActive(ambiencePlayer.IsActive);
        offText.SetActive(!ambiencePlayer.IsActive);
    }

    public void Toggle() {
        ambiencePlayer.SetIsActive(!ambiencePlayer.IsActive);
        UpdateText();
    }
}
