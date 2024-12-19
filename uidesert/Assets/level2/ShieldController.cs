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
        // Handle blocking animation
        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetBool("IsBlocking", true);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            animator.SetBool("IsBlocking", false);
        }
    }
}


