using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerActivity {
    bool IsFinished { get; }
    void Begin(Player player);
    void Update(Player player);
    void Cancel(Player player);
}

public class PlayerController : MonoBehaviour {
    
    [SerializeField] private LayerMask groundMask;

    private ObjectSelector selector;
    private Player player;

    private IPlayerActivity currentActivity;

    private void Awake() {
        player = GetComponent<Player>();
        selector = GetComponent<ObjectSelector>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (selector.Selected != null) {
                var selectedPickUp = selector.Selected.GetComponent<PickUpable>();
                ExecuteActivity(new PlayerPickUpObjectActivity(selectedPickUp));
            } else {
                var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(mouseRay, out var hit, 100f, groundMask)) {
                    ExecuteActivity(new PlayerNavigateToPointActivity(hit.point));
                }
            }
        }

        if (currentActivity != null) {
            currentActivity.Update(player);

            if (currentActivity.IsFinished) {
                currentActivity = null;
            }
        }
    }

    private void ExecuteActivity(IPlayerActivity activity) {
        if (currentActivity != null) {
            currentActivity.Cancel(player);
        }

        currentActivity = activity;
        currentActivity.Begin(player);
    }

}
