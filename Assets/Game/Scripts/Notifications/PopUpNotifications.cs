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

    public void SendNotification(Suggestion suggestion) {
        OnShowNotification?.Invoke(suggestion);
    }

}
