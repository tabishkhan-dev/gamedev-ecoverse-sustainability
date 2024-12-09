using UnityEngine;

public class RockCollisionHandler : MonoBehaviour
{
    public int damageAmount = 10; // Damage applied to the player
    public ParticleSystem rockFragmentEffect; // Particle system for rock breaking effect

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Check if the rock collides with the player
        {
            // Apply damage to the player
            HealthManager healthManager = collision.gameObject.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.TakeDamage(damageAmount); // Apply damage from rock collision
            }

            // Trigger the rock-breaking effect
            BreakRock();
        }
    }

    void BreakRock()
    {
        // Play the particle effect
        if (rockFragmentEffect != null)
        {
            // Detach the particle system to let it play independently
            ParticleSystem effectInstance = Instantiate(rockFragmentEffect, transform.position, transform.rotation);
            effectInstance.Play();
            Destroy(effectInstance.gameObject, effectInstance.main.duration); // Destroy after the effect finishes
        }

        // Destroy the rock object
        Destroy(gameObject);
    }
}
