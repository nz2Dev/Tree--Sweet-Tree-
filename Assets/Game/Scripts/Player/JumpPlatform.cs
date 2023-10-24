using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class JumpPlatform : MonoBehaviour {

    [SerializeField] private UnityEvent OnPlayerOnTop;
    [SerializeField] private UnityEvent OnEmptyOnTop;
    [SerializeField] private bool initIsActive = true;

    public bool IsActive { get; private set; }
    public Transform jumpStartPoint;
    public Transform jumpEndPoint;

    public bool IsPlayerOnTop { get; private set; }
    
    private void Start() {
        SetActive(initIsActive);
    }

    public void SetActive(bool active) {
        IsActive = active;
    }

    public void SetPlayerOnTop(bool isOnTop) {
        IsPlayerOnTop = isOnTop;
        
        if (isOnTop && OnPlayerOnTop != null) {
            OnPlayerOnTop.Invoke();
        }
        if (!isOnTop && OnEmptyOnTop != null) {
            OnEmptyOnTop.Invoke();
        }
    }

}
