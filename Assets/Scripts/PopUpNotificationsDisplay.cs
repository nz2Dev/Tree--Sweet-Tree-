using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpNotificationsDisplay : MonoBehaviour {

    [SerializeField] private PopUpNotifications notifications;
    [SerializeField] private TMP_Text text;

    private Canvas canvas;
    private float startShowingTime;
    private float displayDuration = 1;

    private void Awake() {
        canvas = GetComponent<Canvas>();
        notifications.OnShowNotification += NotificationsOnShowNotification;
        canvas.enabled = false;
    }

    private void NotificationsOnShowNotification(string message, float duration) {
        startShowingTime = Time.time;
        displayDuration = duration;
        text.text = message;
        canvas.enabled = true;
    }

    private void Update() {
        UpdateRotation();
        UpdateShowingTime();
    }

    private void UpdateShowingTime() {
        if (startShowingTime + displayDuration < Time.time) {
            canvas.enabled = false;
        }
    }

    private void UpdateRotation() {
        var camera = Camera.main;
        transform.rotation = camera.transform.rotation;
    }

}
