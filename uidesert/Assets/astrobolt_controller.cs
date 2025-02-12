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

    private int isVoiceControlActive = 0; // Track control mode
    //private string voiceCommand = ""; // Stores last voice command
    private bool isRunningFwd = false; // Track whether the player is running forward
    private bool isRunningBack = false; // Track whether the player is running backward

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Initialize movement direction
        Vector3 moveDirection = Vector3.zero;

        isVoiceControlActive = PlayerPrefs.GetInt("Voice"); // Read Voice Mode

        if(isVoiceControlActive == 1)
        {
            HandleVoiceControl();  // Use Voice Commands
        }
        else
        {
            HandleKeyboardControl();  // Use Keyboard
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
                    stop_turning();
                }
                else if (command.Contains("left"))
                {
                    RotateLeft();
                }
                else if (command.Contains("right") || command.Contains("write"))
                {
                    RotateRight();
                }
                else if (command.Contains("jump"))
                {
                    Jump();
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

        void HandleKeyboardControl()
        {

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                MoveForward();
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                MoveBackward();
            }
            else
            {
                stop_turning();
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                RotateLeft();
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                RotateRight();
            }
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }
        }



        // Handle forward movement (Up Arrow or W)
        void MoveForward()
        {
            moveDirection = transform.forward; // Move in the forward direction

            // Sprinting functionality
            transform.Translate(moveDirection * sprintSpeed * Time.deltaTime, Space.World);
            animator.SetBool("isSprinting", true); // Sprinting animation
        }
        void MoveBackward()
        {
            moveDirection = -transform.forward;
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
            animator.SetBool("isTurning", true);  // Activate animation
        }
        void stop_turning()
        {
            animator.SetBool("isSprinting", false);
            animator.SetBool("isTurning", false); // Stop turning animation
        }

        // Handle left rotation (A or Left Arrow)
        void RotateLeft()
        {
            currentRotation -= rotationAngle; // Decrease rotation by 30 degrees
            transform.rotation = Quaternion.Euler(0, currentRotation, 0); // Apply rotation
        }

        // Handle right rotation (D or Right Arrow)
        void RotateRight()
        {
            currentRotation += rotationAngle; // Increase rotation by 30 degrees
            transform.rotation = Quaternion.Euler(0, currentRotation, 0); // Apply rotation
        }

        // Handle jumping with Spacebar key
        void Jump()
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