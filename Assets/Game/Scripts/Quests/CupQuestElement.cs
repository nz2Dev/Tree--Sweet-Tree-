using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CupQuestElement : MonoBehaviour {

    private GameObject defaultGraphics;
    private GameObject manipulationGraphics;

    private Material isManipulatedMaterial;
    private Material inSpotMaterial;
    private bool isManipulationState;

    private void Awake() {
        defaultGraphics = GetComponentInChildren<MeshRenderer>().gameObject;
    }

    private void Start() {
        manipulationGraphics = Instantiate(defaultGraphics, Vector3.zero, Quaternion.identity);
        manipulationGraphics.transform.SetParent(transform, false);
        manipulationGraphics.transform.localScale = Vector3.one * 1.2f;
        manipulationGraphics.transform.localPosition = defaultGraphics.transform.localPosition;
        Destroy(manipulationGraphics.GetComponent<SelectionStateOutlineActivator>());
        Destroy(manipulationGraphics.GetComponent<Outline>());
        Destroy(manipulationGraphics.GetComponent<BoxCollider>());

        isManipulatedMaterial = Resources.Load("Materials/Transparent Emissive Gray", typeof(Material)) as Material;
        manipulationGraphics.GetComponent<MeshRenderer>().materials = new Material[] {isManipulatedMaterial};
        inSpotMaterial = Resources.Load("Materials/Transparent Emissive Accent", typeof(Material)) as Material;

        UpdateStateObjects();
    }

    public void SetIsManipulationVisuals() {
        isManipulationState = true;
        UpdateStateObjects();
    }

    private void UpdateStateObjects() {
        defaultGraphics.SetActive(!isManipulationState);
        manipulationGraphics.SetActive(isManipulationState);
    }

    public void SetIsInSpotVisuals(bool isInSpot) {
        manipulationGraphics.GetComponent<MeshRenderer>().material = isInSpot ? inSpotMaterial : isManipulatedMaterial;
    }

    public void SetSealed() {
        isManipulationState = false;
        UpdateStateObjects();
        defaultGraphics.GetComponentInChildren<BoxCollider>().enabled = false; // so SelectableObject stop beeing selected
    }

    public void Reset() {
        isManipulationState = false;
        UpdateStateObjects();
    }
}
