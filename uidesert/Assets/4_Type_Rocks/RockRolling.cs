using UnityEngine;

public class RockMovement : MonoBehaviour
{
    public Transform target; // Reference to the character
    public float force = 50f; // Adjust force as needed

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        rb.AddForce(direction * force);
    }
}
