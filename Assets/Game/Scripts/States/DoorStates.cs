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

    private SelectableObject selectable;

    private State currentState;

    private void Awake() {
        selectable = GetComponent<SelectableObject>();
    }

    private void Start() {
        SetState(State.Activator);
    }

    public void SetState(State state) {
        currentState = state;
        switch (state) {
            case State.Activator:
                selectable.enabled = true;
                selectable.SetIsDetectable(true);
                break;
            case State.Quest:
                selectable.enabled = false;
                selectable.SetIsDetectable(false);
                break;
            case State.Stationar:
                selectable.enabled = false;
                selectable.SetIsDetectable(false);
                break; 
        }
    }

}
