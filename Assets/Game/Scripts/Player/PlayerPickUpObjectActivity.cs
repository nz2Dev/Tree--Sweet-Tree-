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
        state = State.Aproaching;
        if (targetPickUp.gameObject.name == "Candle") {
            var playerToPickUp = targetPickUp.transform.position - player.transform.position;
            if (Vector3.Distance(Vector3.zero, new Vector3(playerToPickUp.x, 0, playerToPickUp.z)) < 2) {
                player.ActivatePickUp(targetPickUp);
                state = State.PickingUp;
            } 
        } else {
            player.ActivateNavigation(targetPickUp.transform.position);
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
