using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovanetsCharacter : MonoBehaviour {
    
    [SerializeField] private float walkAnimUnitsPerCycle = 1;
    [SerializeField] private GameObject bagVisuals;
    [SerializeField] private Transform handsLocation;
    [SerializeField] private Transform bagLocation;
    [SerializeField] private bool resetPosition;
    [SerializeField] private bool applyBuiltinRootMotion;

    private Animator animator;
    private int bagLayerIndex;
    private HovanetsAudio audioPlayer;

    public Transform HandsLocation => handsLocation;
    public Transform BagLocation => bagLocation;

    private void Awake() {
        animator = GetComponent<Animator>();
        bagLayerIndex = animator.GetLayerIndex("Bag Layer");
        audioPlayer = GetComponent<HovanetsAudio>();
    }

    private void OnAnimatorMove() {
        if (applyBuiltinRootMotion) {
            animator.ApplyBuiltinRootMotion();    
        }
    }

    private void LateUpdate() {
        if (resetPosition) {
            transform.localPosition = Vector3.zero;
        }
    }

    public void PlayJump() {
        animator.SetTrigger("Jump");
    }

    public void SetIsWalking(bool isWalking) {
        animator.SetBool("Walking", isWalking);
    }

    public void SetWalkMotionSpeed(float motionSpeed) {
        // animSpeed = motionSpeed / walkAnimUnitsPerCycle
        animator.SetFloat("WalkSpeed", motionSpeed / walkAnimUnitsPerCycle);
    }

    public void SetBagEquiped(bool equiped) {
        bagVisuals.SetActive(equiped);
        animator.SetLayerWeight(bagLayerIndex, equiped ? 1.0f : 0.0f);
    }

    private void OnLeftFootDown() {
        audioPlayer.PlayLeftFoot();
    }

    private void OnRightFootDown() {
        audioPlayer.PlayRightFoot();
    }
    
}
