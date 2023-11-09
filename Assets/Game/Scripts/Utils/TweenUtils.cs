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

public struct SequenceState {
    public bool active;
    public float startTime;
    public float startDelay;
    public float duration;
}

public struct TransformCapture {
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
}

public static class TweenUtils {
    
    public static TransformCapture CaptureTransforms(Transform transform) {
        return new TransformCapture {
            position = transform.position,
            rotation = transform.rotation,
            localScale = transform.localScale,
        };
    }

    public static void TweenAll(Transform target, TransformCapture start, Transform end, float t) {
        target.position = Vector3.Lerp(start.position, end.position, t);
        target.rotation = Quaternion.Lerp(start.rotation, end.rotation, t);
        target.localScale = Vector3.Lerp(start.localScale, end.localScale, t);
    }

    public static void TweenDeltaDirection(Transform target, Vector3 position, Vector3 direction, float deltaScale, float t) {
        target.position = position + direction * (t * deltaScale);
    }

    public static SequenceState StartSequence(float duration, float startDelay = 0.0f) {
        return new SequenceState {
            active = true,
            startTime = Time.time,
            startDelay = startDelay,
            duration = duration
        };
    }

    public static bool TryUpdateSequence(SequenceState sequence, out float progress) {
        progress = -1;
        var delayEndTime = sequence.startTime + sequence.startDelay;
        if (Time.time > delayEndTime) {
            var sequenceStartTime = delayEndTime;
            var sequenceEndTime = delayEndTime + sequence.duration; 
            if (Time.time < sequenceEndTime) {
                progress = (Time.time - sequenceStartTime) / sequence.duration;
                return true;
            }
        }
        return false;
    } 

    public static bool TryFinishSequence(ref SequenceState sequence) {
        if (sequence.active && Time.time > sequence.startTime + sequence.startDelay + sequence.duration) {
            sequence.active = false;
            return true;
        }
        return false;
    }

    public static TweenState StartTween(Transform target, Transform destination, float duration, AnimationCurve curve = null) {
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

    public static bool TryFinishTween(ref TweenState state) {
        if (!state.active) {
            return false;
        }

        var endTime = state.startTime + state.duration;
        if (Time.time < endTime) {
            var progress = (Time.time - state.startTime) / state.duration;
            var progressScale = progress;
            if (state.curve != null) {
                progressScale = state.curve.Evaluate(progress);
            }
            state.target.position = Vector3.Lerp(state.startPosition, state.endPosition, progressScale);
            state.target.rotation = Quaternion.Lerp(state.startRotation, state.endRotation, progressScale);
            state.target.localScale = Vector3.Lerp(state.startScale, state.endScale, progressScale);
            return false;
        } else {
            state.active = false;
            return true;
        }
    }

    public static DeltaTweenState StartDeltaTween(Transform target, Vector3 direction, float scale, float duration, AnimationCurve curve = null) {
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

    public static bool TryFinishDeltaTween(ref DeltaTweenState state) {
        if (!state.active) {
            return false;
        }

        var endTime = state.startTime + state.duration;
        if (Time.time < endTime) {
            var progress = (Time.time - state.startTime) / state.duration;
            var progressScale = progress;
            if (state.curve != null) {
                progressScale = state.curve.Evaluate(progress);
            }
            
            var deltaValue = state.direction * (progressScale * state.scale);
            state.target.position = state.startPosition + deltaValue;
            return false;
        } else {
            state.active = false;
            return true;
        }
    }

}
