using UnityEngine;

public class PlayerController_Level2 : MonoBehaviour
{
    public float sprintSpeed = 15f;         // Speed for sprinting forward
    public float backwardSpeed = 7f;       // Speed for moving backward
    public float jumpForce = 30f;          // Vertical force for the long jump
    public float longJumpForwardForce = 40f; // Forward force for the long jump
    public float turnSpeed = 5f;           // Speed of rotation
    private bool isGrounded = true;        // Checks if the player is on the ground

    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Handle forward and backward movement
        HandleMovement();

        // Handle long elevated jump
        HandleLongJump();
    }

    private void HandleMovement()
    {
        // Forward Sprinting (Up Arrow)
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            Vector3 moveDirection = transform.forward;
            transform.Translate(moveDirection * sprintSpeed * Time.deltaTime, Space.World);

            // Trigger sprinting animation
            animator.SetBool("isSprinting", true);
        }
        // Backward Movement (Down Arrow)
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            Vector3 moveDirection = -transform.forward; // Move backward
            transform.Translate(moveDirection * backwardSpeed * Time.deltaTime, Space.World);

            // Stop sprinting animation for backward movement
            animator.SetBool("isSprinting", false);
        }
        else
        {
            // Stop sprinting animation when no movement keys are pressed
            animator.SetBool("isSprinting", false);
        }

        // Handle Left Rotation (A or Left Arrow)
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.down * turnSpeed);
        }

        // Handle Right Rotation (D or Right Arrow)
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up * turnSpeed);
        }
    }

    private void HandleLongJump()
    {
        // Long Elevated Jump (LeftShift + Space)
        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.UpArrow) && isGrounded)
        {
            isGrounded = false; // Prevent further jumps until grounded
            animator.SetTrigger("Jump"); // Trigger jump animation

            // Apply combined vertical and forward force
            Vector3 jumpDirection = Vector3.up * jumpForce + transform.forward * longJumpForwardForce;
            rb.AddForce(jumpDirection, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player is grounded
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Allow jumping again
        }
    }
}
