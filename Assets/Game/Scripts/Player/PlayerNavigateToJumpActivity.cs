using UnityEngine;

public class PlayerNavigateToJumpActivity : IPlayerActivity
{

    private JumpPlatform jumpPlatform;

    private bool startedJump;

    public PlayerNavigateToJumpActivity(JumpPlatform jumpPlatform) {
        this.jumpPlatform = jumpPlatform;
    }

    public bool IsFinished { get; private set; }

    public void Begin(Player player) {
        if (Vector3.Distance(player.transform.position, jumpPlatform.jumpStartPoint.position) < 0.5f) {
            player.ActivateJump(jumpPlatform.transform.position);
        } else {
            player.ActivateNavigation(jumpPlatform.jumpStartPoint.position);
        }
    }

    public void Update(Player player) {
        if (player.GetRemainingNavigationDistance() < 0.1f) {
            player.StopNavigation();
            if (!startedJump) {
                startedJump = true;
                player.ActivateJump(jumpPlatform.transform.position);
            }
        }

        IsFinished = startedJump && !player.IsInJump();
    }

    public void Cancel(Player player) {
    }
}
