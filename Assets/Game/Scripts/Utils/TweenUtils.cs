using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TweenState {
    public bool active;
    public float startTime;
    public float duration;
    public Transform target;
    public Vector3 startPosition;
    public Quaternion startRotation;
    public Vector3 startScale;
    public Vector3 endPosition;
    public Quaternion endRotation;
    public Vector3 endScale;
}

public struct DeltaTweenState {
    public bool active;
    public float startTime;
    public float duration;
    public AnimationCurve curve;
    public Transform target;
    public Vector3 startPosition;
    public Vector3 direction;
    public float scale;
}

public static class TweenUtils {
    
    public static TweenState StartLerpTween(Transform startTransform, Transform destination, float duration) {
        return new TweenState {
            active = true,
            startTime = Time.time,
            duration = duration,
            target = startTransform,
            startPosition = startTransform.position,
            startRotation = startTransform.rotation,
            startScale = startTransform.localScale,
            endPosition = destination.position,
            endRotation = destination.rotation,
            endScale = destination.localScale
        };
    }

    public static bool TryMoveLerpTowardFinish(ref TweenState item) {
        if (!item.active) {
            return false;
        }

        var endTime = item.startTime + item.duration;
        if (Time.time < endTime) {
            var progress = (Time.time - item.startTime) / item.duration;
            item.target.position = Vector3.Lerp(item.startPosition, item.endPosition, progress);
            item.target.rotation = Quaternion.Lerp(item.startRotation, item.endRotation, progress);
            item.target.localScale = Vector3.Lerp(item.startScale, item.endScale, progress);
            return false;
        } else {
            item.active = false;
            return true;
        }
    }

    public static DeltaTweenState StartCurveDeltaTween(Transform target, Vector3 direction, float scale, float duration, AnimationCurve curve) {
        return new DeltaTweenState {
            active = true,
            startTime = Time.time,
            duration = duration,
            curve = curve,
            target = target,
            startPosition = target.position,
            direction = direction,
            scale = scale
        };
    }

    public static bool TryFinishCurveDeltaTween(ref DeltaTweenState state) {
        if (!state.active) {
            return false;
        }

        var endTime = state.startTime + state.duration;
        if (Time.time < endTime) {
            var progress = (Time.time - state.startTime) / state.duration;
            var scaleByDurationValue = state.curve.Evaluate(progress) * state.scale;
            var deltaValue = state.direction * scaleByDurationValue;
            state.target.position = state.startPosition + deltaValue;
            return false;
        } else {
            state.active = false;
            return true;
        }
    }

}
