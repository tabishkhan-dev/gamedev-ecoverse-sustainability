using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Armature_wing|Rotation_fan2 (1)"); // Replace with actual animation clip name
    }
}

