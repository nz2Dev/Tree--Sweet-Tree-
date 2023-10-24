using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Suggestion {
    public Sprite emotion;
    public float emotionDuration;
    public Sprite hint;
    public float hintDuration;
}

public class PopUpNotifications : MonoBehaviour {
    
    public event Action<Suggestion> OnShowNotification;
    public event Action<Suggestion> OnSuggestionClicked;
    public event Action<Suggestion> OnSuggestionEmotionEnd;
    public event Action<Suggestion> OnSuggestionHintEnd;

    public void SendNotification(Suggestion suggestion) {
        OnShowNotification?.Invoke(suggestion);
    }

    public void OnClickEmmotion(Suggestion suggestion) {
        OnSuggestionClicked?.Invoke(suggestion);
    }

    public void OnEmotionEnd(Suggestion suggestion) {
        OnSuggestionEmotionEnd?.Invoke(suggestion);
    }

    public void OnHintEnd(Suggestion suggestion) {
        OnSuggestionHintEnd?.Invoke(suggestion);
    }

}
