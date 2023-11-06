using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchStates : MonoBehaviour {

    public enum State {
        Activation,
        Starter,
        Manipulatable,
        Stationar
    }

    [SerializeField] private GameObject activationState;
    [SerializeField] private ActivationObject activator;
    [SerializeField] private GameObject starterState;
    [SerializeField] private SelectableObject starter;
    [SerializeField] private GameObject manipulatableState;
    [SerializeField] private GameObject manipulatedBench;
    [SerializeField] private GameObject stationarState;
    [SerializeField] private State initState;

    public ActivationObject Activator => activator;
    public SelectableObject Starter => starter;
    public GameObject Manipulated => manipulatedBench;

    private void Start() {
        SetState(initState);
    }

    private void OnValidate() {
        SetState(initState);
    }

    public void SetState(State state) {
        activationState.SetActive(state == State.Activation);
        starterState.SetActive(state == State.Starter);
        manipulatableState.SetActive(state == State.Manipulatable);
        stationarState.SetActive(state == State.Stationar);
    }

    public void SetManipulatedSnappedColor(Color snappedColor) {
        manipulatedBench.GetComponentInChildren<Renderer>().material.color = snappedColor;
    }
}
