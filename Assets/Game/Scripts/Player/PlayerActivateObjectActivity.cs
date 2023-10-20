using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActivateObjectActivity : IPlayerActivity {

    private ActivationObject activationObject;

    public PlayerActivateObjectActivity(ActivationObject activationObject) {
        this.activationObject = activationObject;
    }

    public bool IsFinished { get; private set; }

    public void Begin(Player player) {
        player.ActivateNavigation(activationObject.ActivationPoint.position);
        if (activationObject.TryGetComponent<SelectableObject>(out var selectable)) {
            selectable.Highlight();
        }
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
        if (activationObject.TryGetComponent<SelectableObject>(out var selectable)) {
            selectable.StopHighlighting();
        }
    }

}
