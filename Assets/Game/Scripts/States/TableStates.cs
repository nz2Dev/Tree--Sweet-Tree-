using UnityEngine;

public class TableStates : MonoBehaviour {

    public enum State {
        JumpPlatform,
        QuestActivation,
        Stationar
    }
    
    [SerializeField] private PickUpable candlePickupable;
    [SerializeField] private JumpPlatform jumpPlatform;
    [SerializeField] private ActivationObject activation;
    [SerializeField] private CupQuestController questcontroller;
    [SerializeField] private State initState;
    [SerializeField] private bool debugStateTransition = false;

    private State currentState;

    private void Awake() {
        candlePickupable.OnConsumedEvent += StateOnCandleConsumed;
        activation.OnActivated += StateOnActivateQuest;
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
