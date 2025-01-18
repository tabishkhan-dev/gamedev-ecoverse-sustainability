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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetBool("IsBlocking", true);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            animator.SetBool("IsBlocking", false);
        }
    }
}
