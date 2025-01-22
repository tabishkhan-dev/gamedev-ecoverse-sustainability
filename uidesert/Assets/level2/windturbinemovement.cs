using UnityEngine;

public class windturbinemovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animation anim = GetComponent<Animation>();
        if (anim != null)
        {
            anim.Play("Armature_wing|Rotation_Slow_fan");
        } 
    }

    
}
