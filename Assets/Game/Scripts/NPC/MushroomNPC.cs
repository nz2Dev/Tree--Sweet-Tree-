using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MushroomNPC : MonoBehaviour {

    [SerializeField] private Suggestion thirstySuggestion;
    [SerializeField] private Suggestion needWaterSuggestion;
    [SerializeField] private CinemachineVirtualCamera vcam;

    private Animator characterAnimator;
    private PopUpNotifications npcNotifications;

    private void Awake() {
        characterAnimator = GetComponentInChildren<Animator>();
        npcNotifications = GetComponent<PopUpNotifications>();
        npcNotifications.OnSuggestionClicked += NPCNotificationOnSuggestionClicked;
        npcNotifications.OnSuggestionHintEnd += NPCNotificationOnSuggestionHintEnd;
    }

    private void NPCNotificationOnSuggestionHintEnd(Suggestion suggestion) {
        var isNeedWaterSuggestion = suggestion.hint == needWaterSuggestion.hint;
        if (isNeedWaterSuggestion) {
            OnDeactivate();
        }
    }

    private void NPCNotificationOnSuggestionClicked(Suggestion suggestion) {
        var isNeedWaterSuggestion = suggestion.hint == needWaterSuggestion.hint;
        if (isNeedWaterSuggestion) {
            OnDeactivate();
        }
    }

    public void OnActivated(Player player) {
        vcam.m_Priority = 11;
        characterAnimator.SetTrigger("Active");
        player.ReceiveNotification(thirstySuggestion);

        player.GetComponent<PopUpNotifications>().OnSuggestionClicked -= PlayerNotificationOnSuggestionClicked;
        player.GetComponent<PopUpNotifications>().OnSuggestionClicked += PlayerNotificationOnSuggestionClicked;
    }

    private void OnDeactivate() {
        characterAnimator.SetTrigger("Sleep");
        vcam.m_Priority = 9;
    }

    private void PlayerNotificationOnSuggestionClicked(Suggestion suggestion) {
        var isSendedNotification = suggestion.emotion == thirstySuggestion.emotion;
        if (isSendedNotification) {
            npcNotifications.SendNotification(needWaterSuggestion);
        }
    }

}
