using UnityEngine;

public class PlayerNavigateToJumpActivity : IPlayerActivity {

    private JumpPlatform jumpPlatform;

    private bool startedJump;

    public PlayerNavigateToJumpActivity(JumpPlatform jumpPlatform) {
        this.jumpPlatform = jumpPlatform;
    }

    public bool IsFinished { get; private set; }

    public void Begin(Player player) {
        player.ActivateNavigation(jumpPlatform.jumpStartPoint.position);
    }

    public void Update(Player player) {
        if (player.GetRemainingNavigationDistance() < 0.1f) {
            player.StopNavigation();
            if (!startedJump) {
                startedJump = true;
                player.ActivateJump(jumpPlatform);
            }
        }

        IsFinished = startedJump && !player.IsInJump();
    }

    public void Cancel(Player player) {
    }
}
