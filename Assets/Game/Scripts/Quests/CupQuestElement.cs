using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CupQuestElement : MonoBehaviour {

    private GameObject defaultGraphics;
    private GameObject manipulationGraphics;

    private bool isManipulationState;

    private void Awake() {
        defaultGraphics = GetComponentInChildren<MeshRenderer>().gameObject;
    }

    private void Start() {
        manipulationGraphics = Instantiate(defaultGraphics, Vector3.zero, Quaternion.identity);
        manipulationGraphics.transform.SetParent(transform, false);
        manipulationGraphics.transform.localScale = Vector3.one * 1.2f;
        Destroy(manipulationGraphics.GetComponent<SelectionStateOutlineActivator>());
        Destroy(manipulationGraphics.GetComponent<Outline>());
        Destroy(manipulationGraphics.GetComponent<BoxCollider>());

        var material = Resources.Load("Materials/Transparent Emissive Gray", typeof(Material)) as Material;
        manipulationGraphics.GetComponent<MeshRenderer>().materials = new Material[] {material};

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
    
}
