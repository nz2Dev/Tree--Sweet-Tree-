using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpNotifications : MonoBehaviour {
    
    public event Action<string, float> OnShowNotification;

    public void SendNotification(string text, float duration) {
        OnShowNotification?.Invoke(text, duration);
    }

}
