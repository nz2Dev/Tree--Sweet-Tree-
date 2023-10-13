using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMovement : MonoBehaviour
{
    public CharacterController _controller;

    [SerializeField]
    float playerSpeed = 6f, gravity = -9.81f, jumpPower;
    

    [SerializeField]
    float turnSmoothTime = 0.1f;
    float turnVel;

    [SerializeField]
    Transform CameraTransform;
    Vector3 PlayerVelocity;
    public Transform groundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;

    bool isGrounded;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, GroundDistance, GroundMask);
        if(isGrounded && PlayerVelocity.y < 0)
        {
            PlayerVelocity.y = -2.5f;
        }

        float _horizontal = Input.GetAxisRaw("Horizontal");
        float _vertical = Input.GetAxisRaw("Vertical");
        

        Vector3 direction = new Vector3(_horizontal, 0, _vertical).normalized;
        //Vector3 direction = wasd.ReadValue<Vector3>();

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + CameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVel, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            _controller.Move(moveDirection.normalized * playerSpeed * Time.deltaTime);
        }
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            
            PlayerVelocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
        }

        PlayerVelocity.y += gravity * Time.deltaTime;
        _controller.Move(PlayerVelocity * Time.deltaTime);

    }



}
