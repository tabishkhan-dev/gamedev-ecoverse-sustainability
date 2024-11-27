using UnityEngine;

public class RockCollisionHandler : MonoBehaviour
{
    public GameObject rockFragmentPrefab; // Prefab for rock fragments or destroyed rock
    public int damageAmount = 10; // Damage to apply to the character

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Ensure the character has the tag "Player"
        {
            // Apply damage to the character
            HealthManager healthManager = collision.gameObject.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.TakeDamage(damageAmount);
            }

            // Destroy or break the rock
            BreakRock();
        }
    }

    void BreakRock()
    {
        if (rockFragmentPrefab != null)
        {
            Instantiate(rockFragmentPrefab, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
