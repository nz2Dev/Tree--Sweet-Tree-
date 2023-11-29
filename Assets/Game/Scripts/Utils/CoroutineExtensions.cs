using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineExtensions {
    
    public static void StartDelayedActionCallback(this MonoBehaviour monoBehaviour, float delay, Action callback) {
        monoBehaviour.StartCoroutine(DelayedActionRunner(delay, callback));
    }

    private static IEnumerator DelayedActionRunner(float delay, Action action) {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

}
