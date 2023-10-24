using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationToggle : MonoBehaviour {

    [SerializeField] private string openAnimationName;
    [SerializeField] private string closeAnimationName;

    private Animation thisAnimation;
    private bool open;

    private void Awake() {
        thisAnimation = GetComponent<Animation>();
    }

    public void TogglePlay() {
        thisAnimation.Play(open ? closeAnimationName : openAnimationName);
        open = !open;
    }

}
