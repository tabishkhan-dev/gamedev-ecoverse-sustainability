using UnityEngine;

public class PlayerController_Level2 : MonoBehaviour
{
    public float sprintSpeed = 15f;         // Speed for sprinting forward
    public float backwardSpeed = 7f;       // Speed for moving backward
    public float jumpForce = 30f;          // Vertical force for the long jump
    public float longJumpForwardForce = 40f; // Forward force for the long jump
    public float turnSpeed = 5f;           // Speed of rotation
    private bool isGrounded = true;        // Checks if the player is on the ground
    private int isVoiceControlActive = 0; // Track control mode
    private bool isRunningFwd = false; // Track whether the player is running forward
    private bool isRunningBack = false; // Track whether the player is running backward

    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isVoiceControlActive = PlayerPrefs.GetInt("Voice"); // Read Voice Mode

        if(isVoiceControlActive == 1)
        {
            HandleVoiceControl();  // Use Voice Commands
        }
        else
        {
            HandleKeyboardControl();  // Use Keyboard
        }
    }

    void HandleVoiceControl()
    {
        string command = VoiceProcessorDemo.latestTranscription; // Get latest transcribed command

        //if (string.IsNullOrEmpty(command)) return; // If no command, do nothing

        if (VoiceProcessorDemo.isNewTranscription) // ✅ Process only when new transcription arrives
        {
            if (command.Contains("run"))
            {
                MoveForward();
                isRunningFwd = true; // Set running flag
                isRunningBack = false; // Set running flag
            }
            else if (command.Contains("back"))
            {
                isRunningFwd = false; // Set running flag
                isRunningBack = true; // Set running flag
                MoveBackward();
            }
            else if (command.Contains("stop"))
            {
                isRunningFwd = false; // Set running flag
                isRunningBack = false; // Set running flag
                animator.SetBool("isSprinting", false);
            }
            else if (command.Contains("left"))
            {
                transform.Rotate(Vector3.down * turnSpeed);
            }
            else if (command.Contains("right") || command.Contains("write"))
            {
                transform.Rotate(Vector3.up * turnSpeed);
            }
            else if (command.Contains("jump"))
            {
                LongJump();
            }
        }
        VoiceProcessorDemo.isNewTranscription = false; // ✅ Reset flag after processing
        // ✅ Keep running if the player said "run" until "stop" is spoken
        if (isRunningFwd)
        {
            MoveForward();
        }
        if (isRunningBack)
        {
            MoveBackward();
        }
    }

    void MoveForward()
    {
        Vector3 moveDirection = transform.forward;
        transform.Translate(moveDirection * sprintSpeed * Time.deltaTime, Space.World);

        // Trigger sprinting animation
        animator.SetBool("isSprinting", true);
    }

    void MoveBackward()
    {
        Vector3 moveDirection = -transform.forward; // Move backward
        transform.Translate(moveDirection * backwardSpeed * Time.deltaTime, Space.World);

        // Stop sprinting animation for backward movement
        animator.SetBool("isSprinting", false);
    }

    private void LongJump()
    {
        // Long Elevated Jump (LeftShift + Space)
        if (isGrounded)
        {
            isGrounded = false; // Prevent further jumps until grounded
            animator.SetTrigger("Jump"); // Trigger jump animation

            // Apply combined vertical and forward force
            Vector3 jumpDirection = Vector3.up * jumpForce + transform.forward * longJumpForwardForce;
            rb.AddForce(jumpDirection, ForceMode.Impulse);
        }
    }

    void HandleKeyboardControl()
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
        if (Input.GetKey(KeyCode.Space)  && isGrounded)
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