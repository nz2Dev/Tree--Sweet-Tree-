using System;
using UnityEngine;

public class TableStates : MonoBehaviour {

    public enum State {
        JumpPlatform,
        QuestActivation,
        Stationar
    }
    
    [SerializeField] private JumpPlatform benchPlatform;
    [SerializeField] private JumpPlatform jumpPlatform;
    [SerializeField] private ActivationObject activation;
    [SerializeField] private CupQuestController questcontroller;
    [SerializeField] private State initState;
    [SerializeField] private bool debugStateTransition = false;

    private State currentState;

    private void Awake() {
        activation.OnActivated += StateOnActivateQuest;
        questcontroller.OnExit += StateOnQuestExit;
        benchPlatform.OnPlayerOnTopChanged += StateOnBenchPlayerOnTopChanged;
        jumpPlatform.OnPlayerOnTopChanged += StateOnTablePlayerOnTopChanged;
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

    private void StateOnTablePlayerOnTopChanged(bool isPlayerOnTable) {
        if (!isPlayerOnTable) {
            SetState(questcontroller.HasFinished ? State.JumpPlatform : State.QuestActivation);
        }
    }

    private void StateOnBenchPlayerOnTopChanged(bool isPlayerOnBench) {
        if (isPlayerOnBench) {
            SetState(State.JumpPlatform);
        } else {
            SetState(questcontroller.HasFinished ? State.JumpPlatform : State.QuestActivation);
        }
    }

    private void StateOnActivateQuest() {
        if (CheckCurrentStateIs(State.QuestActivation)) {
            SetState(State.Stationar);
            questcontroller.OnActivated();
        }
    }

    private void StateOnQuestExit() {
        SetState(questcontroller.HasFinished ? State.JumpPlatform : State.QuestActivation);
    }

    private void SetState(State newState) {
        if (debugStateTransition) {
            Debug.Log("Table transition state from " + currentState + " -> " + newState);
        }
        currentState = newState;
        UpdateStateGameObjects();
    }

    private void UpdateStateGameObjects() {
        switch (currentState) {
            case State.JumpPlatform:
                SetObjects(
                    jumpPlatform: true, 
                    activation: false);
                break;

            case State.QuestActivation:
                SetObjects(
                    jumpPlatform: false, 
                    activation: true);
                break;

            case State.Stationar:
                SetObjects(
                    jumpPlatform: false, 
                    activation: false);
                break;
        }
    }

    private void SetObjects(bool jumpPlatform = false, bool activation = false) {
        this.jumpPlatform.gameObject.SetActive(jumpPlatform);
        this.activation.GetComponent<SelectableObject>().SetIsDetectable(activation);
    }

}
