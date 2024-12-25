using UnityEngine;

public class AddPotionHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public int healthValue = 20; // Amount of health restored

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the HealthManager script attached to the player
            HealthManager healthManager = other.GetComponent<HealthManager>();

            if (healthManager != null)
            {
                // Use the AddHealth method to restore health
                healthManager.AddHealth(healthValue);

                // Destroy the potion object
                Destroy(gameObject);
            }
        }
    }



}
