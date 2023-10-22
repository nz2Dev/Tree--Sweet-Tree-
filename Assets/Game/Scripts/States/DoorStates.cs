using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorStates : MonoBehaviour {
    public enum State {
        Activator,
        Quest,
        Stationar
    }

    [SerializeField] private GameObject activatorState;
    [SerializeField] private GameObject emptyState; // QuestState
    [SerializeField] private GameObject stationarState;
    [SerializeField] private State initState;

    private State currentState;

    private void Start() {
        SetState(initState);
    }

    private void OnValidate() {
        SetState(initState);
    }

    public void SetState(State state) {
        currentState = state;
        activatorState.SetActive(state == State.Activator);
        emptyState.SetActive(state == State.Quest);
        stationarState.SetActive(state == State.Stationar);
    }

    public bool IsQuestState() {
        return currentState == State.Quest;
    }
}
