using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuestElementItem {
    public GameObject elementGO;
    public Vector3 initialPosition;
    public bool isInSpot;
}

public class CupAssembler : MonoBehaviour {
    [SerializeField] private Transform elementsLocation;
    [SerializeField] private GameObject assemblyCenter;
    [SerializeField] private float elementsPlacementsOffset = 0.25f;
    [SerializeField] private GameObject assembledCupPickupablePrefab;

    private Vector3 nextElementPlacementPosition;
    private List<QuestElementItem> questElementItems;

    private void Awake() {
        questElementItems = new List<QuestElementItem>();
    }

    private void Start() {
        nextElementPlacementPosition = elementsLocation.position;
        assemblyCenter.SetActive(false);
    }

    public void PutOutNextPiece(GameObject piece) {
        var placementPosition = nextElementPlacementPosition;
        piece.transform.position = placementPosition;

        questElementItems.Add(new QuestElementItem {
            elementGO = piece,
            initialPosition = placementPosition,
            isInSpot = false,
        });

        nextElementPlacementPosition = placementPosition + elementsLocation.forward * elementsPlacementsOffset;
    }

    public bool TryGetAssocicatedPieceItem(GameObject comparisonObject, out QuestElementItem associatedItem) {
        foreach (var questItem in questElementItems) {
            if (questItem.elementGO == comparisonObject) {
                associatedItem = questItem;
                return true;
            }
        }

        associatedItem = default;
        return false;
    }

    public void ClearPieces() {
        foreach (var item in questElementItems) {
            Destroy(item.elementGO);
        }
        questElementItems.Clear();
    }

    public Plane GetAssemblyPlane() {
        return new Plane(Vector3.up, assemblyCenter.transform.position);
    }

    public void SetAsseblyCenterHighlighted(bool highlighted) {
        assemblyCenter.SetActive(highlighted);
    }

    public Vector3 GetAssemblyCenter() {
        return assemblyCenter.transform.position;
    }

    public PickUpable CreateCupPickupable() {
        return Instantiate(assembledCupPickupablePrefab, assemblyCenter.transform.position, Quaternion.identity)
            .GetComponent<PickUpable>();
    }

    public bool IsAllPiecesInSpot() {
        var allInSpot = true;
        if (questElementItems.Count == 0) {
            allInSpot = false;
        }
        foreach (var questElementItem in questElementItems) {
            if (!questElementItem.isInSpot) {
                allInSpot = false;
                break;
            }
        }
        return allInSpot;
    }
}
