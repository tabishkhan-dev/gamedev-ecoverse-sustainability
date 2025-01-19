using UnityEngine;

public class GameOverlevel2 : MonoBehaviour
{
    private HealthManagerLevel2 healthManagerlevel2; // Correct type

    void Start()
    {
        // Get the HealthManagerLevel2 on the same GameObject
        healthManagerlevel2 = GetComponent<HealthManagerLevel2>();
        if (healthManagerlevel2 == null)
        {
            Debug.LogError("HealthManagerLevel2 script not found on Player GameObject!");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Terrain"))
        {
            Debug.Log("Player hit terrain! Triggering game over.");
            healthManagerlevel2.Die(); // Call Die() on the correct type
        }
    }
}


