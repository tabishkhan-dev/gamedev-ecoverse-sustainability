using UnityEngine;


public class GameOverlevel2 : MonoBehaviour
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
        if (collision.gameObject.CompareTag("Terrain"))
        {
            Debug.Log("Player hit terrain! Triggering game over.");
            healthManager.Die();
        }
    }
}

