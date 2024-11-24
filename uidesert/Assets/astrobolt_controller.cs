using UnityEngine;

public class Astrobolt_controller : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.S)) // Hold Left Shift to sprint
        {
            animator.SetBool("isSprinting", true);
        }
        else
        {
            animator.SetBool("isSprinting", false);
        }

        if (Input.GetButtonDown("Jump")) // Spacebar for jump
        {
            animator.SetTrigger("Jump");
        }
    }
}
