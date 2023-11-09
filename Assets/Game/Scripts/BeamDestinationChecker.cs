using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamDestinationChecker : MonoBehaviour {

    [SerializeField] private GameObject errorDestination;

    private TransportableObject transportableObject;

    private SequenceState drainingSequenceState;

    private void Awake() {
        transportableObject = GetComponent<TransportableObject>();
        transportableObject.OnLayedOutAt += (destination) => {
            if (destination == errorDestination) {
                StartDrainingSequence();
            }
        };
    }

    private void Update() {
        UpdateDrainingSequence();
    }

    private void StartDrainingSequence() {
        drainingSequenceState = TweenUtils.StartSequence(0.5f, 0.2f);
    }

    private void UpdateDrainingSequence() {
        if (TweenUtils.TryUpdateSequence(drainingSequenceState, out var progress)) {
            TweenUtils.TweenDeltaDirection(
                transportableObject.transform, 
                errorDestination.transform.position,
                Vector3.down, 1, progress);
        }
        if (TweenUtils.TryFinishSequence(ref drainingSequenceState)) {
            Player.LatestInstance.ActivateGrab(transportableObject);
        }
    }
    
}
