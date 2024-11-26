using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float sprintSpeed = 1500f; // Speed when sprinting
    public float jumpForce = 7f;   // Jumping force
    private bool isGrounded = true; // Check if the player is on the ground

    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Initialize movement speed to 0 (Idle)
        Vector3 moveDirection = Vector3.zero;

        // Get horizontal input (for strafing left/right)
        float horizontalInput = Input.GetAxis("Horizontal"); // -1 for left, 1 for right

        // Check for sprinting input (e.g., Left Shift key)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection += Vector3.forward; // Add forward movement
            moveDirection += Vector3.right * horizontalInput; // Add horizontal movement (left/right)
            
            // Normalize the direction to maintain consistent speed
            moveDirection = moveDirection.normalized * sprintSpeed * Time.deltaTime;

            // Apply movement and trigger sprint animation
            transform.Translate(moveDirection, Space.World);
            animator.SetBool("isSprinting", true); // Trigger sprint animation
        }
        else
        {
            animator.SetBool("isSprinting", false); // Remain idle
        }

        // Trigger the Jump animation and apply jump force when the spacebar is pressed
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isGrounded = false;
            animator.SetTrigger("Jump");
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse); // Apply jump force vertically
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player is back on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
