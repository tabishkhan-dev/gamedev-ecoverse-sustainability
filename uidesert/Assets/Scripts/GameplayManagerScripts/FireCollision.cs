
using UnityEngine;

public class FireCollision : MonoBehaviour
{
    //private ParticleSystem fireParticleSystem;
    private HealthManager healthManager; // Assign this in the Inspector

    void Start()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Find the player
        if (player != null)
        {
            healthManager = player.GetComponent<HealthManager>();
        }

        if (healthManager == null)
        {
            Debug.LogWarning("Failed to find HealthManager on the player.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        
        // Check if the colliding object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Print your name to the console

            if (collision.gameObject.CompareTag("Player"))
            {
                // Call the Die method if the script is found
                bool isFireActive = CheckAllParticleSystems();

                if (isFireActive)
                {
                    if (healthManager != null)
                    {
                        healthManager.Die();
                    }
                    else
                    {
                        Debug.Log("Script not found.");
                    }
                }
            }
        }
    }

    private bool CheckAllParticleSystems()
    {
        // Get all Particle Systems in the parent object and its children
        ParticleSystem[] fireParticleSystems = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in fireParticleSystems)
        {
            // Check if the Particle System is emitting or has active particles
            if (ps.particleCount < 30 && ps.particleCount > 12)
            {
                Debug.Log($"Player dies on particle count : {ps.particleCount}");
                return true; // Return true if any Particle System is active
            }
            Debug.Log($"Fire not burning : {ps.particleCount}");
        }
        
        return false; // Return false if no Particle System is active
    }
}
