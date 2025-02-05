using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 7f;    // Normal movement speed
    public float sprintSpeed = 15f; // Sprinting speed
    public float jumpForce = 30f;   // Jumping force
    private bool isGrounded = true; // Check if the player is on the ground

    private Animator animator;
    private Rigidbody rb;

    public float rotationAngle = 30f; // Amount to rotate left/right per key press
    private float currentRotation = 0f; // Tracks the current rotation of the player

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Initialize movement direction
        Vector3 moveDirection = Vector3.zero;

        // Handle forward movement (Up Arrow or W)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection = transform.forward; // Move in the forward direction

            // Sprinting functionality
            transform.Translate(moveDirection * sprintSpeed * Time.deltaTime, Space.World);
            animator.SetBool("isSprinting", true); // Sprinting animation
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
    {
        moveDirection = -transform.forward;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        animator.SetBool("isTurning", true);  // Activate animation
    }
    else
    {
        animator.SetBool("isSprinting", false);
        animator.SetBool("isTurning", false); // Stop turning animation
    }

        // Handle left rotation (A or Left Arrow)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentRotation -= rotationAngle; // Decrease rotation by 30 degrees
            transform.rotation = Quaternion.Euler(0, currentRotation, 0); // Apply rotation
        }

        // Handle right rotation (D or Right Arrow)
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentRotation += rotationAngle; // Increase rotation by 30 degrees
            transform.rotation = Quaternion.Euler(0, currentRotation, 0); // Apply rotation
        }

        // Handle jumping with Shift key
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isGrounded = false; // Temporarily prevent jumping again until grounded
            animator.SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player is back on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Reset grounded state on touching the ground
        }
    }
}