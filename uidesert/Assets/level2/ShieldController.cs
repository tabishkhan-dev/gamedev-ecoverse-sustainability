using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Handle shield blocking when the S key is pressed
        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetBool("IsBlocking", true); // Trigger shield block
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            animator.SetBool("IsBlocking", false); // Stop shield block
        }
    }
}



