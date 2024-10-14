using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    private float walkSpeed = 8.0f;
    private float sprintSpeed = 9.0f;
    // [SerializeField] float acceleration = 18f;

    public float groundDrag;

    [Header("Jumping")]
    private float jumpForce = 15.0f;
    private float jumpCooldown = 0.5f;
    private float airMultiplier = 0.0f;
    bool readyToJump;

    [Header("Keybinds")]
    private KeyCode jumpKey = KeyCode.Space;
    private KeyCode sprintKey = KeyCode.LeftShift;
    private KeyCode crouchKey = KeyCode.LeftControl;


    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.2f;

    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool grounded;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private RaycastHit slopeHit;
    [SerializeField] private bool exitingSlope;

    [SerializeField] private Transform orientation;
    [SerializeField] private GameObject playerModel;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    static public bool dialogue = false;

    [SerializeField] private MovementState state; 

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            playerModel.transform.rotation = orientation.rotation;
        }

        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        Gravity();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        if (!dialogue)
        {
            MovePlayer();
        }
    }

    private void MyInput()
    {
        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += orientation.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= orientation.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= orientation.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += orientation.right;
        }

        moveDirection.Normalize();

        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        StateHandler();
    }

    private void StateHandler()
    {
        // mode - sprinting
        if (grounded && Input.GetKey(sprintKey)) 
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        //mode - walking 
        if (grounded) 
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // Mode - Air
        else 
        {
            state = MovementState.air;
        }
    }

    void Gravity()
    {
        // rb.AddForce(0, -gravity, 0);
        rb.AddForce(Physics.gravity);
    }


    private void MovePlayer()
    {
        // On slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // On ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // In air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // Turn off gravity while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); 
    }
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.2f)) 
        {
            Debug.DrawRay(transform.position, Vector3.down, Color.blue); 

            float angle = Vector3.Angle(Vector3.up, slopeHit.normal); 
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()  
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
