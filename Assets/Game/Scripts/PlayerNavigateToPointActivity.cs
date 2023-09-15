using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNavigateToPointActivity : IPlayerActivity {   

    private Vector3 navigationPoint;

    public bool IsFinished { get; private set; }

    public PlayerNavigateToPointActivity(Vector3 navigationPoint) {
        this.navigationPoint = navigationPoint;
    }

    public void Begin(Player player) {
        player.ActivateNavigation(navigationPoint);
    }


    public void Update(Player player) {
        IsFinished = player.GetRemainingNavigationDistance() < float.Epsilon;
    }

    public void Cancel(Player player) {
        player.StopNavigation();
    }
}
