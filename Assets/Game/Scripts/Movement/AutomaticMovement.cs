using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutomaticMovement : MonoBehaviour {
    
    [SerializeField] private float jumpMaxHight = 1f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float rotationSpeed = 5;

    private NavMeshAgent navMeshAgent;
    private bool jumpStarted;
    private float jumpStartTime;
    private Vector3 jumpStartPosition;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoTraverseOffMeshLink = false;
        navMeshAgent.updateRotation = false;
    }

    private void Update() {
        UpdateRotation();
        UpdateLinkJump();
        DrawDestinationDebug();
    }

    public void PrintDebug() {
        Debug.Log("path - has?: " + navMeshAgent.hasPath
            + " isPending?: " + navMeshAgent.pathPending
            + " isStale?: " + navMeshAgent.isPathStale
            + " status:" + navMeshAgent.pathStatus
            + " remaining distance: " + navMeshAgent.remainingDistance
            + " nextLink: " + navMeshAgent.nextOffMeshLinkData);
    }

    public void MoveTo(Vector3 destination) {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(destination);
    }

    public float GetRemainingDistance() {
        if (navMeshAgent.pathPending) {
            return float.PositiveInfinity;
        } else {
            return navMeshAgent.remainingDistance;
        }
    }

    public float GetCurrentSpeed() {
        return navMeshAgent.velocity.magnitude;
    }

    public void StopMovement() {
        navMeshAgent.destination = navMeshAgent.nextPosition;
    }

    private void UpdateRotation() {
        if (navMeshAgent.velocity.sqrMagnitude > 0) {
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                    Quaternion.LookRotation(navMeshAgent.velocity, Vector3.up), 
                    Time.deltaTime * rotationSpeed);
        }
    }

    private void DrawDestinationDebug() {
        if (navMeshAgent.hasPath) {
            Debug.DrawRay(navMeshAgent.destination, Vector3.up, Color.red);
        }
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
