using UnityEngine;

public class RockBreak : MonoBehaviour
{
    public ParticleSystem rockFragmentEffect; // Assign the Particle System in the Inspector

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the rock collides with the player
        if (collision.gameObject.CompareTag("Player")) // Ensure your player has the "Player" tag
        {
            // Play the rock fragment effect
            if (rockFragmentEffect != null)
            {
                rockFragmentEffect.Play();
            }

            // Optionally destroy the rock after triggering the effect
            Destroy(gameObject, 2f); // Delays destruction to let the effect finish
        }
    }
}
