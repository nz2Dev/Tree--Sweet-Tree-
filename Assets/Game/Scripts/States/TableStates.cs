using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TableStates : MonoBehaviour {

    public enum State {
        JumpPlatform,
        QuestActivation,
        Stationar
    }
    
    [SerializeField] private PickUpable candlePickupable;
    [SerializeField] private GameObject jumpPlatformState;
    [SerializeField] private ActivationObject questActivator;
    [SerializeField] private CupQuestController questcontroller;
    [SerializeField] private GameObject questActivationState;
    [SerializeField] private GameObject stationarState;
    [SerializeField] private State previewState;

    private State currentState;
    private State stateStack;

    private void Awake() {
        candlePickupable.OnConsumedEvent += StateOnCandleConsumed;
        questActivator.OnActivated += StateOnActivateQuest;
        questcontroller.OnExit += StateOnQuestExit;
    }

    private void Start() {
        SetState(State.JumpPlatform);
    }

    private void OnValidate() {
        SetState(previewState);
    }

    private bool TryAssertState(State state) {
        Assert.AreEqual(currentState, state);
        return currentState == state;
    }

    private void StateOnCandleConsumed() {
        if (TryAssertState(State.JumpPlatform)) {
            SetState(State.QuestActivation);
        }
    }

    private void StateOnActivateQuest() {
        if (TryAssertState(State.QuestActivation)) {
            SetState(State.Stationar);
            questcontroller.OnActivated();
        }
    }

    private void StateOnQuestExit() {
        if (TryAssertState(State.Stationar)) {
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
