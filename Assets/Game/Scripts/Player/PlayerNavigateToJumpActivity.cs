using UnityEngine;

public class PlayerNavigateToJumpActivity : IPlayerActivity {

    private JumpPlatform jumpPlatform;
    private bool force;

    private bool startedJump;

    public PlayerNavigateToJumpActivity(JumpPlatform jumpPlatform, bool force = false) {
        this.jumpPlatform = jumpPlatform;
        this.force = force;
    }

    public bool IsFinished { get; private set; }

    public void Begin(Player player) {
        if (force) {
            startedJump = true;
            player.ActivateJump(jumpPlatform);
        } else {
            player.ActivateNavigation(jumpPlatform.jumpStartPoint.position);
        }
    }

    public void Update(Player player) {
        if (!startedJump && player.GetRemainingNavigationDistance() < 0.1f) {
            startedJump = true;
            player.StopNavigation();
            player.ActivateJump(jumpPlatform);
        }

        IsFinished = startedJump && !player.IsInJump();
    }

    public void Cancel(Player player) {
    }
}
