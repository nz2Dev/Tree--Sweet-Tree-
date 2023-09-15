using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate : MonoBehaviour {

    [SerializeField] private int frameRate = 40;

    private void Update() {
        if (Application.targetFrameRate != frameRate) {
            Application.targetFrameRate = frameRate;
        }
    }

}
