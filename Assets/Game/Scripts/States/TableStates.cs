using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableStates : MonoBehaviour {

    public enum State {
        JumpPlatform,
        QuestActivation
    }
    
    [SerializeField] private GameObject jumpPlatformState;
    [SerializeField] private GameObject questActivationState;
    [SerializeField] private State previewState;

    private State state;

    private void Start() {
        state = State.JumpPlatform;
        UpdateStateGameObjects();
    }

    private void OnValidate() {
        state = previewState;
        UpdateStateGameObjects();
    }

    private void UpdateStateGameObjects() {
        jumpPlatformState.SetActive(state == State.JumpPlatform);
        questActivationState.SetActive(state == State.QuestActivation);
    }

}
