using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingStates : MonoBehaviour {

    public enum State {
        Activator,
        Quest,
        Stationar
    }

    private SelectableObject selectable;
    
    [SerializeField] private State initState;

    private void Awake() {
        selectable = GetComponent<SelectableObject>();
    }

    private void Start() {
        SetState(initState);
    }

    public void SetState(State state) {
        switch (state) {
            case State.Activator:
                SetComponents(selection: true);
                break;
            case State.Quest:
                SetComponents(selection: false);
                break;
            case State.Stationar:
                SetComponents(selection: false);
                break;
        }
    }

    private void SetComponents(bool selection = false) {
        selectable.SetIsDetectable(selection);
    }

}
