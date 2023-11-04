using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LerpState {
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

public static class LerpUtils {
    
    public static LerpState StartLerp(Transform startTransform, Transform destination, float duration) {
        return new LerpState {
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

    public static bool TryMoveLerpTowardFinish(ref LerpState item) {
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

}
