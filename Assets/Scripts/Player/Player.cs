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

    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    public RaycastHit slopeHit;
    public bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    static public bool dialogue = false;

    public MovementState state; // always stores the current state the player is in

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
        // ground check
        // grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround); //maak  van 0.1f een nieuwe float [SerializeField] float groundDistance = 0.2f; en noem het hier
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
        // Reset movement direction
        moveDirection = Vector3.zero;

        // Check for movement keys
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

        // Normalize the movement direction to ensure consistent speed
        moveDirection.Normalize();

        // When to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // State handling based on key inputs
        StateHandler();
    }

    private void StateHandler()
    {
        // mode - sprinting
        if (grounded && Input.GetKey(sprintKey)) // if player is grounded and is presiing sprintkey we want to set the state Walking to Sprinting
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        //mode - walking 
        if (grounded) //if player is grounded but not pressing sprint set state to walkSpeed
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // Mode - Air
        else // if player is not grounded  and not pressing sprint set state to air
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

        // limiting speed on ground or air
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

        // reset y velocity
        //rb.velocity = new Vector3(rb.velocity.x, 5f, rb.velocity.z); // consistant jumping same heigt every time 

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);     //en dit zorg for inconsistant jumping dus soms spring je hoger dan een andere keer
    }
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.2f)) // out slopeHit this stores the object that we hit in the slopeHit Variable
        {
            Debug.DrawRay(transform.position, Vector3.down, Color.blue); // !!!!!! testing pruposes

            float angle = Vector3.Angle(Vector3.up, slopeHit.normal); // calculate how steep the slope is 
            return angle < maxSlopeAngle && angle != 0; // we want the bool to return true if the angle is smaller then oure maxSlope Angle & not 0
        }

        return false; // if the raycast doe not hit annyhing return false 
    }

    private Vector3 GetSlopeMoveDirection() // project oure moveDirection with the angle of the slope(if slope 40degrees moveDirection wil be 40degrees) os you wont walk in the slope but on it 
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
