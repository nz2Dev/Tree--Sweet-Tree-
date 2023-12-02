using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScallingEffect : MonoBehaviour {

    [SerializeField] private float effectDuration = 0.5f;
    [SerializeField] private AnimationCurve scaleDownCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve scaleUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool controlGameObjectActiveState = true;

    private SequenceState effectSequenceState;
    private AnimationCurve curve;
    private bool endVisibility;
    private float fromScale;
    private float toScale;

    private Action onFinishedCallback;

    public void ScaleDown() {
        ScaleDown(null);
    }

    public void ScaleDown(Action onFinished) {
        this.onFinishedCallback = onFinished;
        StartScaling(fromScale: 1.0f, toScale: 0.0f, endVisibility: false, scaleDownCurve);
    }

    public void ScaleUp() {
        StartScaling(fromScale: 0.5f, toScale: 1.0f, endVisibility: true, scaleUpCurve);
    }

    private void Update() {
        UpdateScaling();
    }

    private void StartScaling(in float fromScale, in float toScale, in bool endVisibility, in AnimationCurve curve) {
        this.endVisibility = endVisibility;
        this.fromScale = fromScale;
        this.toScale = toScale;
        this.curve = curve;
        effectSequenceState = TweenUtils.StartSequence(effectDuration);
        gameObject.SetActive(true);
    }

    private void UpdateScaling() {
        if (TweenUtils.TryUpdateSequence(effectSequenceState, out var progress)) {
            var scaleFactor = curve.Evaluate(progress);
            var scale = fromScale + (toScale - fromScale) * scaleFactor;
            transform.localScale = Vector3.one * scale;
        }
        if (TweenUtils.TryFinishSequence(ref effectSequenceState)) {
            transform.localScale = Vector3.one * toScale;
            if (controlGameObjectActiveState) {
                gameObject.SetActive(endVisibility);
            }
            onFinishedCallback?.Invoke();
        }
    }

}
