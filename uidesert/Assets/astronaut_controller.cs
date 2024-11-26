using UnityEngine;


public class astronaut_controller : MonoBehaviour
{
   private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W)) 
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (Input.GetButtonDown("Jump")) // Spacebar for jump
        {
            animator.SetTrigger("Jump");
        }
    }
}
