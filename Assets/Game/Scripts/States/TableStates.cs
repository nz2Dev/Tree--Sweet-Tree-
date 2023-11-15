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
    [SerializeField] private State initState;
    [SerializeField] private bool debugStateTransition = false;

    private State currentState;

    private void Awake() {
        candlePickupable.OnConsumedEvent += StateOnCandleConsumed;
        questActivator.OnActivated += StateOnActivateQuest;
        questcontroller.OnExit += StateOnQuestExit;
    }

    private void Start() {
        SetState(initState);
    }

    private bool CheckCurrentStateIs(State state) {
        if (currentState != state) {
            Debug.LogError("State " + state + " was expected, instead of: " + currentState);
        }
        return currentState == state;
    }

    private void StateOnCandleConsumed() {
        if (CheckCurrentStateIs(State.JumpPlatform)) {
            SetState(State.QuestActivation);
        }
    }

    private void StateOnActivateQuest() {
        if (CheckCurrentStateIs(State.QuestActivation)) {
            SetState(State.Stationar);
            questcontroller.OnActivated();
        }
    }

    private void StateOnQuestExit() {
        if (CheckCurrentStateIs(State.Stationar)) {
            SetState(State.QuestActivation);
        }
    }

    private void SetState(State newState) {
        if (debugStateTransition) {
            Debug.Log("Table transition state from " + currentState + " -> " + newState);
        }
        currentState = newState;
        UpdateStateGameObjects();
    }

    private void UpdateStateGameObjects() {
        jumpPlatformState.SetActive(currentState == State.JumpPlatform);
        questActivationState.SetActive(currentState == State.QuestActivation);
        stationarState.SetActive(currentState == State.Stationar);
    }

}
