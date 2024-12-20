using UnityEngine;

public class CrystalCollisionHandler : MonoBehaviour
{
    public int damageAmount = 3; // Damage applied to the player
    private bool hasCollided = false; // Flag to prevent multiple damage applications

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the player
        if (!hasCollided && collision.gameObject.CompareTag("Player"))
        {
            // Get the HealthManager from the player
            HealthManager healthManager = collision.gameObject.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                // Apply damage to the player
                healthManager.TakeDamage(damageAmount);
            }

            // Prevent further damage from this collision
            hasCollided = true;

        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Reset the collision flag when the player exits the crystal's collision area
        if (collision.gameObject.CompareTag("Player"))
        {
            hasCollided = false;
        }
    }

}
