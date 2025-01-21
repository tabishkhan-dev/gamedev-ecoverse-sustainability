using UnityEngine;


public class GameOverLava : MonoBehaviour
{
    private HealthManager healthManager;

    void Start()
    {
        // Get the HealthManager on the same GameObject
        healthManager = GetComponent<HealthManager>();
        if (healthManager == null)
        {
            Debug.LogError("HealthManager script not found on Player GameObject!");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Lava"))
        {
            Debug.Log("Player hit lava! Triggering game over.");
            healthManager.Die();
        }
    }
}

