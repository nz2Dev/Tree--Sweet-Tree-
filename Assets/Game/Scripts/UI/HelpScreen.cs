using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelpScreen : MonoBehaviour, IPointerClickHandler {
    
    [SerializeField] private ScallingEffect helpButton;
    [SerializeField] private Image background;
    [SerializeField] private ScallingEffect helpImageStateEffect;
    [SerializeField] private GameObject closeButton;

    private bool isOpen;

    private void Awake() {
        background.enabled = false;
        helpImageStateEffect.gameObject.SetActive(false);
        closeButton.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Close();
        }
    }

    public void Open() {
        ShowInternaly();
    }

    private void ShowInternaly() {
        isOpen = true;
        background.enabled = true;
        background.CrossFadeAlpha(0.6f, 0.2f, true);
        helpImageStateEffect.ScaleUp();
        this.StartDelayedActionCallback(0.2f, () => closeButton.SetActive(true));
    }

    public void Close() {
        if (isOpen) {
            HideInternaly();
        }
    }

    private void HideInternaly() {
        isOpen = false;
        background.CrossFadeAlpha(0.0f, 0.2f, true);
        this.StartDelayedActionCallback(0.2f, () => background.enabled = false);
        helpImageStateEffect.ScaleDown();
        helpButton.ScaleUp();
        closeButton.SetActive(false);
    }

    public void OnCloseApplicationClick() {
        Application.Quit();
    }

    public void OnPointerClick(PointerEventData eventData) {
        Close();
    }
}
