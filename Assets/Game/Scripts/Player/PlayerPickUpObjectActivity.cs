using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpObjectActivity : IPlayerActivity {

    public enum State {
        Idle,
        Aproaching,
        PickingUp,
        Finished
    }

    private readonly PickUpable targetPickUp;
    private State state;

    public PlayerPickUpObjectActivity(PickUpable targetPickUp) {
        this.targetPickUp = targetPickUp;
        this.state = State.Idle;
    }

    public bool IsFinished => state == State.Finished;

    public PickUpable TargetPickUpable => targetPickUp;

    public void Begin(Player player) {        
        if (targetPickUp.PickUpPlatform != null) {
            if (targetPickUp.PickUpPlatform.IsActive && targetPickUp.PickUpPlatform == player.PlatformUnder) {
                player.ActivatePickUp(targetPickUp);
                state = State.PickingUp; 
            } else {
                player.ActivateJump(null);
                state = State.Finished;
            }
        } else {
            player.ActivateNavigation(targetPickUp.transform.position);
            state = State.Aproaching;
        }

        if (targetPickUp.TryGetComponent<SelectableObject>(out var selectable)) {
            selectable.Highlight();
        }
    }

    public void Update(Player player) {
        switch (state) {
            case State.Aproaching:
                if (player.GetRemainingNavigationDistance() < targetPickUp.PickUpRadius) {
                    player.StopNavigation();
                    player.ActivatePickUp(targetPickUp);
                    state = State.PickingUp;
                }
                break;
            
            case State.PickingUp:
                if (player.HasPickedUp()) {
                    player.HandlePickedUp();
                    state = State.Finished;
                } 
                break;
        }
    }

    public void Cancel(Player player) {
        player.StopNavigation();

        if (state != State.Finished && player.IsPickingUp()) {
            player.CancelPickUp();
        }

        if (targetPickUp.TryGetComponent<SelectableObject>(out var selectable)) {
            selectable.StopHighlighting();
        }
    }

}
