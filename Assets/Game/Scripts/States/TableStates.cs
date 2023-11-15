using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableStates : MonoBehaviour {

    public enum State {
        JumpPlatform,
        QuestActivation,
        Stationar
    }
    
    [SerializeField] private PickUpable candlePickupable;
    [SerializeField] private GameObject jumpPlatformState;
    [SerializeField] private GameObject questActivationState;
    [SerializeField] private GameObject stationarState;
    [SerializeField] private State previewState;

    private State currentState;
    private State stateStack;

    private void Awake() {
        candlePickupable.OnConsumedEvent += StateOnCandleConsumed;
    }

    private void Start() {
        SetState(State.JumpPlatform);
    }

    private void OnValidate() {
        SetState(previewState);
    }

    private void StateOnCandleConsumed() {
        if (currentState == State.JumpPlatform) {
            SetState(State.QuestActivation);
        }
    }

    public void SetState(State newState) {
        stateStack = default;
        currentState = newState;
        UpdateStateGameObjects();
    }

    public void PushState(State newState) {
        stateStack = currentState;
        currentState = newState;
        UpdateStateGameObjects();
    }

    public void PopState() {
        currentState = stateStack;
        stateStack = default;
        UpdateStateGameObjects();
    }

    private void UpdateStateGameObjects() {
        jumpPlatformState.SetActive(currentState == State.JumpPlatform);
        questActivationState.SetActive(currentState == State.QuestActivation);
        stationarState.SetActive(currentState == State.Stationar);
    }

}
