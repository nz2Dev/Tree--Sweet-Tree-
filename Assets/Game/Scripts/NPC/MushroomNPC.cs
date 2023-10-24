using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomNPC : MonoBehaviour {

    [SerializeField] private Suggestion thirstySuggestion;
    [SerializeField] private Suggestion needWaterSuggestion;

    private Animator characterAnimator;
    private PopUpNotifications npcNotifications;

    private void Awake() {
        characterAnimator = GetComponentInChildren<Animator>();
        npcNotifications = GetComponent<PopUpNotifications>();
        npcNotifications.OnSuggestionClicked += NPCNotificationOnSuggestionClicked;
        npcNotifications.OnSuggestionEmotionEnd += NPCNotificationOnSuggestionEmptionEnd;
    }

    private void NPCNotificationOnSuggestionEmptionEnd(Suggestion suggestion) {
        var isNeedWaterSuggestion = suggestion.hint == needWaterSuggestion.hint;
        if (isNeedWaterSuggestion) {
            characterAnimator.SetTrigger("Sleep");
        }
    }

    private void NPCNotificationOnSuggestionClicked(Suggestion suggestion) {
        var isNeedWaterSuggestion = suggestion.hint == needWaterSuggestion.hint;
        if (isNeedWaterSuggestion) {
            characterAnimator.SetTrigger("Sleep");
        }
    }

    public void OnActivated(Player player) {
        characterAnimator.SetTrigger("Active");
        player.ReceiveNotification(thirstySuggestion);

        player.GetComponent<PopUpNotifications>().OnSuggestionClicked -= PlayerNotificationOnSuggestionClicked;
        player.GetComponent<PopUpNotifications>().OnSuggestionClicked += PlayerNotificationOnSuggestionClicked;
    }

    private void PlayerNotificationOnSuggestionClicked(Suggestion suggestion) {
        var isSendedNotification = suggestion.emotion == thirstySuggestion.emotion;
        if (isSendedNotification) {
            npcNotifications.SendNotification(needWaterSuggestion);
        }
    }

}
