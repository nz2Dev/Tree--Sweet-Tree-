using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbMovePlatformActivity : IPlayerActivity {

    private MovePlatform movePlatform;

    public PlayerClimbMovePlatformActivity(MovePlatform movePlatform) {
        this.movePlatform = movePlatform;
    }

    public bool IsFinished { get; private set; }

    public void Begin(Player player) {
        player.ActivateMovePlatform(movePlatform);
    }

    public void Update(Player player) {
        if (!IsFinished && !player.IsPlatforming()) {
            IsFinished = true;
        }
    }

    public void Cancel(Player player) {
        player.CancelMovePlatform();
    }
}
