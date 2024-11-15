using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to the character
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get movement input
        float move = Input.GetAxis("Vertical");
        bool isRunning = Mathf.Abs(move) > 0.1f;

        // Check if the sprint button (e.g., Left Shift) is pressed
        bool isSprinting = isRunning && Input.GetKey(KeyCode.LeftShift);

        // Set the parameters in the Animator
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isSprinting", isSprinting);

        // Trigger the Jump animation when the spacebar is pressed
        if (Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("Jump");
        }
    }
}
