using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovanetsCharacter : MonoBehaviour {
    
    [SerializeField] private float walkAnimUnitsPerCycle = 1;
    [SerializeField] private GameObject bagVisuals;
    [SerializeField] private Transform handsLocation;
    [SerializeField] private Transform bagLocation;

    private Animator animator;
    private int bagLayerIndex;

    public Transform HandsLocation => handsLocation;
    public Transform BagLocation => bagLocation;

    private void Awake() {
        animator = GetComponent<Animator>();
        bagLayerIndex = animator.GetLayerIndex("Bag Layer");
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
    
}
