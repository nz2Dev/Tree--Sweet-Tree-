using UnityEngine;

public struct TweenState {
    public bool active;
    public float startTime;
    public float duration;
    public AnimationCurve curve;
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
    
    public static TweenState StartLerpTween(Transform target, Transform destination, float duration) {
        return new TweenState {
            active = true,
            startTime = Time.time,
            duration = duration,
            curve = AnimationCurve.Linear(0, 0, 1, 1),
            target = target,
            startPosition = target.position,
            startRotation = target.rotation,
            startScale = target.localScale,
            endPosition = destination.position,
            endRotation = destination.rotation,
            endScale = destination.localScale
        };
    }

    public static TweenState StartCurveTween(Transform target, Transform destination, float duration, AnimationCurve curve) {
        return new TweenState {
            active = true,
            startTime = Time.time,
            duration = duration,
            curve = curve,
            target = target,
            startPosition = target.position,
            startRotation = target.rotation,
            startScale = target.localScale,
            endPosition = destination.position,
            endRotation = destination.rotation,
            endScale = destination.localScale
        };
    }

    public static bool TryFinishLerpTween(ref TweenState state) {
        if (!state.active) {
            return false;
        }

        var endTime = state.startTime + state.duration;
        if (Time.time < endTime) {
            var progress = (Time.time - state.startTime) / state.duration;
            state.target.position = Vector3.Lerp(state.startPosition, state.endPosition, progress);
            state.target.rotation = Quaternion.Lerp(state.startRotation, state.endRotation, progress);
            state.target.localScale = Vector3.Lerp(state.startScale, state.endScale, progress);
            return false;
        } else {
            state.active = false;
            return true;
        }
    }

    public static bool TryFinishCurveTween(ref TweenState state) {
        if (!state.active) {
            return false;
        }

        var endTime = state.startTime + state.duration;
        if (Time.time < endTime) {
            var progress = state.curve.Evaluate((Time.time - state.startTime) / state.duration);
            state.target.position = Vector3.Lerp(state.startPosition, state.endPosition, progress);
            state.target.rotation = Quaternion.Lerp(state.startRotation, state.endRotation, progress);
            state.target.localScale = Vector3.Lerp(state.startScale, state.endScale, progress);
            return false;
        } else {
            state.active = false;
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
