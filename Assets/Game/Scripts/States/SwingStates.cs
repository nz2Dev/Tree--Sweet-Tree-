using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingStates : MonoBehaviour {

    public enum State {
        Activator,
        Quest
    }

    [SerializeField] private GameObject activatorState;
    [SerializeField] private GameObject questState;
    [SerializeField] private State initState;

    private void Start() {
        SetState(initState);
    }

    private void OnValidate() {
        SetState(initState);
    }

    public void SetState(State state) {
        activatorState.SetActive(state == State.Activator);
        questState.SetActive(state == State.Quest);
    }

}
