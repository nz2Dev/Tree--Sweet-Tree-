using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivationObject : MonoBehaviour {
    [SerializeField] private float activationRadius = 2f;
    [SerializeField] private Transform activationPoint;
    [SerializeField] private JumpPlatform activationPlatform;
    [SerializeField] private UnityEvent OnActivatedEvent;
    [SerializeField] private UnityEvent<Player> OnActivatedByPlayerEvent;
    [SerializeField] private bool debugActivateOnAwake = false;

    public float ActivationRadius => activationRadius;
    public JumpPlatform ActivationPlatform => activationPlatform;
    public Transform ActivationPoint => activationPoint;

    public event Action OnActivated;

    private void Awake() {
        if (activationPoint == null) {
            activationPoint = transform;
        }
    }

    private void Start() {
        if (debugActivateOnAwake) {
            Activate(Player.LatestInstance);
        }
    }

    public void Activate(Player player) {
        OnActivated?.Invoke();
        
        if (OnActivatedEvent != null) {
            OnActivatedEvent.Invoke();
        }

        if (OnActivatedByPlayerEvent != null) {
            OnActivatedByPlayerEvent.Invoke(player);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        var position = activationPoint == null ? transform.position : activationPoint.position;
        Gizmos.DrawWireSphere(position, activationRadius);        
    }
#endif

}
