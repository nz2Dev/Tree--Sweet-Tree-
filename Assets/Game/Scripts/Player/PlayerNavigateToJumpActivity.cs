using UnityEngine;

public class PlayerNavigateToJumpActivity : IPlayerActivity {

    private JumpPlatform jumpPlatform;

    private bool startedJump;

    public PlayerNavigateToJumpActivity(JumpPlatform jumpPlatform) {
        this.jumpPlatform = jumpPlatform;
    }

    public bool IsFinished { get; private set; }

    public void Begin(Player player) {
        if (Vector3.Distance(jumpPlatform.jumpStartPoint.position, player.transform.position) < 0.4f) {
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
