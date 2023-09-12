using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PreciseMovement : MonoBehaviour {
    [SerializeField] private LayerMask solidObjectsMask;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpMaxHight = 1f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float rotationSpeed = 3;

    private NavMeshAgent navMeshAgent;
    private bool flying;
    private float jumpStartY;
    private float jumpStartTime;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        var inputMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        var inputInWorldSpace = Camera.main.transform.TransformDirection(inputMoveDirection);
        var inputProjected = Vector3.ProjectOnPlane(inputInWorldSpace, Vector3.up);

        Move(inputProjected);

        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }

        UpdateJump();
    }

    public void Move(Vector3 direction) {
        if (direction.sqrMagnitude > 0f) {
            navMeshAgent.isStopped = true;
            transform.position = transform.position + direction * speed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), Time.deltaTime * rotationSpeed);
        }
    }

    public void Jump() {
        flying = true;
        navMeshAgent.updatePosition = false;
        jumpStartTime = Time.time;
        jumpStartY = transform.position.y;
    }

    private void UpdateJump() {
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
