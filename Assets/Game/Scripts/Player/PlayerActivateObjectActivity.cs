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
        if (activationObject.ActivationPlatform != null) {
            if (activationObject.ActivationPlatform.IsActive && activationObject.ActivationPlatform == player.PlatformUnder) {
                activationObject.Activate(player);
                IsFinished = true; 
            } else {
                player.ActivateJump(null);
                IsFinished = true;
            }
        } else {
            if (Vector3.Distance(player.transform.position, activationObject.ActivationPoint.position) > activationObject.ActivationRadius) {
                player.ActivateNavigation(activationObject.ActivationPoint.position);
            } else {
                activationObject.Activate(player);
                IsFinished = true;
            }
        }

        if (activationObject.TryGetComponent<SelectableObject>(out var selectable)) {
            selectable.Highlight();
        }
    }

    public void Update(Player player) {
        // double activation might occure if we check for activation in the Begin method because of "IsFinished" state of system design 
        if (IsFinished) {
            return;
        }

        if (Vector3.Distance(player.transform.position, activationObject.ActivationPoint.position) < activationObject.ActivationRadius) {
            player.StopNavigation();
            activationObject.Activate(player);
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
