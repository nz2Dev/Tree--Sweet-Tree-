using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SelectableObject))]
[RequireComponent(typeof(ActivationObject))]
public class Bench : MonoBehaviour {

    public enum State {
        Activation,
        Manipulatable,
        Stationar
    }

    [SerializeField] private BenchManipulatorController manipulatorController;
    [SerializeField] private JumpPlatform stationarPlatform;
    [SerializeField] private Transform rotationPivot;
    [SerializeField] private Color snappedColor = Color.blue;
    [SerializeField] private State initState;

    private SelectableObject selectable;
    private ActivationObject activator;
    private NavMeshObstacle obstacle;
    private State currentState;

    private Material defaultMaterial;

    public SelectableObject Selectable => selectable;

    private void Awake() {
        selectable = GetComponent<SelectableObject>();
        obstacle = GetComponentInChildren<NavMeshObstacle>();
        activator = GetComponent<ActivationObject>();
        activator.OnActivated += StateOnBenchActivated;

        defaultMaterial = GetComponentInChildren<Renderer>().material;
    }

    private void Start() {
        SetState(initState);
    }

    public void RotateBase(float rotationDegrees) {
        rotationPivot.localRotation *= Quaternion.AngleAxis(rotationDegrees, Vector3.right);
    }

    public void SetBaseRotation(float rotationDegrees) {
        rotationPivot.localRotation = Quaternion.AngleAxis(rotationDegrees, Vector3.right);
    }

    public float GetBaseRotationInDegrees() {
        return Vector3.SignedAngle(rotationPivot.parent.up, rotationPivot.up, rotationPivot.parent.right);
        // return Quaternion.Angle(Quaternion.identity, rotationPivot.localRotation);
    }

    public void SetManipulatableMoveable() {
        GetComponentInChildren<Renderer>().material = Resources.Load("Materials/Transparent Emissive Gray", typeof(Material)) as Material;
        selectable.SetIsDetectable(false);
    }

    public void SetManipulatableSnapped() {
        GetComponentInChildren<Renderer>().material.color = snappedColor;
    }

    public void SetManipulatableStationar(bool final) {
        GetComponentInChildren<Renderer>().material = defaultMaterial;
        if (final) {
            StateOnBenchStationar();
        } else {
            SetState(State.Activation);
        }
    }

    private bool CheckCurrentStateIs(State state) {
        if (currentState != state) {
            Debug.LogError("State " + state + " was expected, instead of: " + currentState);
        }
        return currentState == state;
    }

    private void StateOnBenchActivated() {
        if (CheckCurrentStateIs(State.Activation)) {
            SetState(State.Manipulatable);
            manipulatorController.Activate(this);
        }
    }

    private void StateOnBenchStationar() {
        if (CheckCurrentStateIs(State.Manipulatable)) {
            SetState(State.Stationar);
        }
    }

    private void SetState(State state) {
        currentState = state;
        UpdateStateObjects();
    }

    private void UpdateStateObjects() {
        switch (currentState) {
            case State.Activation:
                SetComponents(
                    detectable: true,
                    activatable: true,
                    obstacle: true,
                    platform: false);
                break;
            case State.Manipulatable:
                SetComponents(
                    detectable: true,
                    activatable: false,
                    obstacle: false,
                    platform: false);
                break;
            case State.Stationar:
                SetComponents(
                    detectable: false,
                    activatable: false,
                    obstacle: true,
                    platform: true);
                break;
        }
    }

    private void SetComponents(bool detectable = false, bool activatable = false, bool obstacle = false, bool platform = false) {
        this.obstacle.enabled = obstacle;
        this.activator.SetIsActivatable(activatable);
        this.selectable.SetIsDetectable(detectable);
        this.stationarPlatform.gameObject.SetActive(platform);
    }

}
