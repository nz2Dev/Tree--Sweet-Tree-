using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovanetsCharacter : MonoBehaviour {
    
    [SerializeField] private float walkAnimUnitsPerCycle = 1;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void SetIsWalking(bool isWalking) {
        animator.SetBool("Walking", isWalking);
    }

    public void SetWalkMotionSpeed(float motionSpeed) {
        // animSpeed = motionSpeed / walkAnimUnitsPerCycle
        animator.SetFloat("WalkSpeed", motionSpeed / walkAnimUnitsPerCycle);
    }

}
