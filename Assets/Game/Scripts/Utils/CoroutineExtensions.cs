using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineExtensions {
    
    public static Coroutine StartDelayedActionCallback(this MonoBehaviour monoBehaviour, float delay, Action callback) {
        return monoBehaviour.StartCoroutine(DelayedActionRunner(delay, callback));
    }

    private static IEnumerator DelayedActionRunner(float delay, Action action) {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

}
