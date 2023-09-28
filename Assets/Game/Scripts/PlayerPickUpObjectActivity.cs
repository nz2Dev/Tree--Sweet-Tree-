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
        player.ActivateNavigation(targetPickUp.transform.position);
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
    }

}
