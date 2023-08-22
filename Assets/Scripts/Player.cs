using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {
    
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask solidObjectsMask;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpMaxHight = 1f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private AnimationCurve jumpCurve;

    private NavMeshAgent navMeshAgent;
    private bool flying;
    private float jumpStartTime;
    private float jumpStartY;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");
        var inputVector = new Vector3(horizontalInput, 0, verticalInput);

        if (inputVector.sqrMagnitude > 0f) {
            navMeshAgent.isStopped = true;
            transform.position = transform.position + inputVector * speed * Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0)) {
            navMeshAgent.isStopped = false;
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out var hit, groundMask)) {
                navMeshAgent.SetDestination(hit.point);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            StartJump();
        }

        if (flying) {
            if (jumpStartTime + jumpDuration > Time.time) {
                var jumpTime = Time.time - jumpStartTime;
                var jumpProgress = jumpTime / jumpDuration;
                var jumpHightDelta = jumpCurve.Evaluate(jumpProgress) * jumpMaxHight;
                var desiredPosition = new Vector3(transform.position.x, jumpStartY + jumpHightDelta, transform.position.z);
                transform.position = desiredPosition;
                if (jumpProgress > 0.5f && Physics.CheckSphere(desiredPosition, 0.2f, solidObjectsMask)) {
                    GroundJump();
                }
            } else {
                GroundJump();
            }
        }
    }

    private void StartJump() {
        flying = true;
        navMeshAgent.updatePosition = false;
        jumpStartTime = Time.time;
        jumpStartY = transform.position.y;
    }

    private void GroundJump() {
        flying = false;
        if (NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas)) {
            navMeshAgent.Warp(hit.position);
        } else {
            navMeshAgent.Warp(transform.position);
        }
        navMeshAgent.updatePosition = true;
    }
}
