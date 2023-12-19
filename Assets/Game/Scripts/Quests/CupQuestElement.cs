using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CupQuestElement : MonoBehaviour {

    [SerializeField] private GameObject emptyIndicationGraphic;
    [SerializeField] private ItemSO discoveringItemSO;

    private GameObject defaultGraphics;
    private GameObject manipulationGraphics;

    private Material isManipulatedMaterial;
    private Material inSpotMaterial;
    private bool isManipulationState;

    private bool isDiscovered;
    private Vector3 initialPosition;
    private bool isInSpot;
    private bool isIndicationEnabled;

    public bool IsDiscovered => isDiscovered;
    public ItemSO DiscoveringItemSO => discoveringItemSO;
    public bool IsInSpot => isInSpot;

    private void Awake() {
        defaultGraphics = GetComponentInChildren<MeshRenderer>().gameObject;
        initialPosition = transform.position;
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

    public void ShowQuestIndication() {
        isIndicationEnabled = true;
        UpdateStateObjects();
    }

    public void HideQuestIndication() {
        isIndicationEnabled = false;
        UpdateStateObjects();
    }

    public void SetIsDiscovered() {
        isDiscovered = true;
        UpdateStateObjects();
    }

    public void SetIsManipulationVisuals() {
        isManipulationState = true;
        UpdateStateObjects();
    }

    private void UpdateStateObjects() {
        defaultGraphics.SetActive(!isManipulationState && isDiscovered);
        manipulationGraphics.SetActive(isManipulationState && isDiscovered);
        
        emptyIndicationGraphic.SetActive(isIndicationEnabled && !isDiscovered);
    }

    public void SetIsInSpot(bool isInSpot) {
        this.isInSpot = isInSpot;
        manipulationGraphics.GetComponent<MeshRenderer>().material = isInSpot ? inSpotMaterial : isManipulatedMaterial;
    }

    public void SetSealed() {
        isManipulationState = false;

        UpdateStateObjects();
        defaultGraphics.GetComponentInChildren<BoxCollider>().enabled = false; // so SelectableObject stop beeing selected
    }

    public void Reset() {
        transform.position = initialPosition;

        isManipulationState = false;
        UpdateStateObjects();
    }
}
