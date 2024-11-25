using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float sprintSpeed = 10f; // Speed when sprinting
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
        float currentSpeed = 0f;

        // Check for sprinting input (e.g., Left Shift key)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed; // Set speed to sprint speed
            animator.SetBool("isSprinting", true); // Trigger sprint animation
        }
        else
        {
            animator.SetBool("isSprinting", false); // Remain idle
        }

        // Move the character forward at the determined speed
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

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

