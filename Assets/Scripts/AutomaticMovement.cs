using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutomaticMovement : MonoBehaviour {
    
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float jumpMaxHight = 1f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private AnimationCurve jumpCurve;

    private NavMeshAgent navMeshAgent;
    private bool jumpStarted;
    private float jumpStartTime;
    private Vector3 jumpStartPosition;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoTraverseOffMeshLink = false;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out var hit, groundMask)) {
                MoveTo(hit.point);
            }
        }
        
        UpdateLinkJump();        
    }

    public void MoveTo(Vector3 destination) {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(destination);
    }

    private void UpdateLinkJump() {
        if (navMeshAgent.isOnOffMeshLink) {
            if (!jumpStarted) {
                jumpStarted = true;
                jumpStartTime = Time.time;   
                jumpStartPosition = transform.position;
            }

            if (jumpStartTime + jumpDuration > Time.time) {
                var jumpTime = Time.time - jumpStartTime;
                var jumpProgress = jumpTime / jumpDuration;
                if (jumpProgress < 0.2f) {
                    jumpProgress = 0;
                } else {
                    jumpProgress = 1 - ((1 - jumpProgress) / 0.8f); // remapping progress from 0.2 - 1.0 to 0.0 - 1.0
                }
                
                var jumpStart = jumpStartPosition;
                var jumpHightDelta = jumpCurve.Evaluate(jumpProgress) * jumpMaxHight * Vector3.up;
                var jumpDistanceVector = navMeshAgent.currentOffMeshLinkData.endPos - jumpStart;
                var jumpWidthDelta = jumpDistanceVector * jumpProgress;

                var desiredPosition = jumpStart + jumpHightDelta + jumpWidthDelta;
                transform.position = desiredPosition;
            } else {
                navMeshAgent.transform.position = navMeshAgent.currentOffMeshLinkData.endPos;
                navMeshAgent.CompleteOffMeshLink();
                jumpStarted = false;
            }
        }
    }

}
