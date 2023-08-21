using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {
    
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float speed = 3f;

    private NavMeshAgent navMeshAgent;

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
    }

}
