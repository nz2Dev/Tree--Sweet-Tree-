using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActivateObjectActivity : IPlayerActivity {

    private ActivationObject activationObject;
    private Action onCancel;

    public PlayerActivateObjectActivity(ActivationObject activationObject, Action onCancel) {
        this.activationObject = activationObject;
        this.onCancel = onCancel;
    }

    public bool IsFinished { get; private set; }

    public void Begin(Player player) {
        player.ActivateNavigation(activationObject.ActivationPoint.position);
    }

    public void Update(Player player) {
        if (Vector3.Distance(player.transform.position, activationObject.ActivationPoint.position) < activationObject.ActivationRadius) {
            player.StopNavigation();
            activationObject.Activate();
            IsFinished = true;
        }
    }

    public void Cancel(Player player) {
        player.StopNavigation();
        onCancel?.Invoke();
    }

}
