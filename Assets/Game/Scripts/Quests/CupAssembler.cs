using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CupAssembler : MonoBehaviour {
    [SerializeField] private Transform elementsContainer;
    [SerializeField] private GameObject assemblyCenter;
    [SerializeField] private GameObject assembledCupPickupablePrefab;
    [SerializeField] private CupQuestElement[] questElements;
    [SerializeField] private AudioSource effectsAudioSource;
    [SerializeField] private AudioClip movementAudioClip;
    [SerializeField] private AudioClip rotationAudioClip;
    [SerializeField] private AudioClip applyAudioClip;

    private CupQuestElement activatedQuestElement;
    private Quaternion rotationProgres;
    private bool rotationStage;

    private void Start() {
        assemblyCenter.SetActive(false);
        TurnOffIndication();
    }

    public void TurnOnIndication() {
        foreach (var questElement in questElements) {
            questElement.ShowQuestIndication();
        }
    }

    public void TurnOffIndication() {
        foreach (var questElement in questElements) {
            questElement.HideQuestIndication();
        }
    }

    public bool CanReceivePiece(ItemSO item) {
        foreach (var questElement in questElements) {
            if (!questElement.IsDiscovered && questElement.DiscoveringItemSO == item) {
                return true;
            }
        }

        return false;
    }

    public void DiscoverNextPiece(ItemSO piece) {
        foreach (var questElement in questElements) {
            if (questElement.DiscoveringItemSO == piece) {
                questElement.SetIsDiscovered();
            }
        }
    }

    public bool TryDetectActivatablePieceElement(GameObject comparisonObject, out CupQuestElement pieceElement) {
        foreach (var questElement in questElements) {
            if (questElement.IsDiscovered && questElement.gameObject == comparisonObject) {
                pieceElement = questElement;
                return true;
            }
        }

        pieceElement = null;
        return false;
    }

    public void ClearPieces() {
        elementsContainer.gameObject.SetActive(false);
    }

    public void ActivateManipulationState(CupQuestElement questElement) {
        activatedQuestElement = questElement;
        activatedQuestElement.SetIsManipulationVisuals();

        activatedQuestElement.transform.position = assemblyCenter.transform.position;
        SetIsRotationStage(true);
    }

    public void MoveManipulated(Vector3 position) {
        activatedQuestElement.transform.position = position;
    }

    public void SetIsRotationStage(bool rotationStage) {
        if (!this.rotationStage && rotationStage) {
            rotationProgres = activatedQuestElement.transform.rotation;
        }

        this.rotationStage = rotationStage;
    }

    public bool IsRotationStage() {
        return this.rotationStage;
    }

    private float rotationStackForSFX;

    public void RotateManipulated(float rotationAmount) {
        var rotationDelta = Quaternion.AngleAxis(rotationAmount, Vector3.up);

        rotationProgres *= rotationDelta;
        if (rotationProgres.eulerAngles.y > 50 && rotationProgres.eulerAngles.y < 70) {
            activatedQuestElement.transform.rotation = Quaternion.Euler(0, 60, 0);
            activatedQuestElement.SetIsInSpot(true);
        } else {
            activatedQuestElement.transform.rotation = rotationProgres;
            activatedQuestElement.SetIsInSpot(false);
        }

        rotationStackForSFX += Mathf.Abs(rotationAmount);
        if (rotationStackForSFX > 5) {
            rotationStackForSFX = 0;
            effectsAudioSource.clip = rotationAudioClip;
            effectsAudioSource.Play();
        }
    }

    public void ResetManipulatedRotationInSpot() {
        activatedQuestElement.SetIsInSpot(false);
    }

    public void HandleManipulationResult() {
        if (activatedQuestElement.IsInSpot) {
            effectsAudioSource.PlayOneShot(applyAudioClip);
            activatedQuestElement.SetSealed();
            SetIsRotationStage(false);
        } else {
            effectsAudioSource.PlayOneShot(movementAudioClip);
            activatedQuestElement.Reset();
        }

        activatedQuestElement = null;
    }

    public void CancelActivated() {
        if (activatedQuestElement != null) {
            SetIsRotationStage(false);
            activatedQuestElement.Reset();
        }
        
        activatedQuestElement = null;
    }

    public bool IsQuestElementActivated() {
        return activatedQuestElement != null;
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
        foreach (var questElementItem in questElements) {
            if (!questElementItem.IsInSpot) {
                return false;
            }
        }
        return true;
    }

}
