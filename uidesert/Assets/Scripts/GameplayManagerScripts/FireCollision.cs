using UnityEngine;

public class FireCollision : MonoBehaviour
{
    private HealthManager healthManager;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            healthManager = player.GetComponent<HealthManager>();
        }
    }

    // This method is triggered when particles collide with another object
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by fire particles!");

            if (healthManager != null)
            {
                healthManager.Die();
            }
        }
    }
}

