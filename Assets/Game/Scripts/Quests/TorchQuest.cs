using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class TorchQuest : MonoBehaviour {

    [SerializeField] private SwingStates swingStates;
    [SerializeField] private ObjectSelector objectSelector;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private float cameraCutDuration = 0.9f;
    [SerializeField] private Transform itemHubTransform;

    private bool activated;

    private void Awake() {
        vcam.m_Priority = 9;
    }

    public void Activate() {
        vcam.m_Priority += 2;
        activated = true;

        StartHideCharacter();
        BindToInventoryEvents();
        swingStates.SetState(SwingStates.State.Quest);
    }

    private void BindToInventoryEvents() {
        Player.LatestInstance.GetComponent<Inventory>().OnItemActivated += InventoryOnItemActivated;
    }

    private void InventoryOnItemActivated(Item item) {
        var itemGO = GameObject.Instantiate(item.prefab, Vector3.zero, Quaternion.identity);
        itemGO.transform.SetParent(itemHubTransform, false);
    }

    private float startHideCharacterTime;

    private void StartHideCharacter() {
        startHideCharacterTime = Time.time;   
    }

    private void OnChangeCharacterVisibility(bool visibility) {
        Player.LatestInstance.GetComponentInChildren<HovanetsCharacter>(true).gameObject.SetActive(visibility);
    }

    private bool applyRegime;

    private void Update() {
        if (activated) {
            if (Time.time > startHideCharacterTime + cameraCutDuration) {
                OnChangeCharacterVisibility(false);
            }

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
                if (!applyRegime) {
                    if (objectSelector.Selected != null && objectSelector.Selected.CompareTag("torchElement")) {
                        objectSelector.Selected.Highlight();
                        applyRegime = true;
                    }
                }
            }
        }
    }
    
}
