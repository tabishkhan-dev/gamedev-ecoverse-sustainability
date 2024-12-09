using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Image healthBarFill;
    public int damageFromRock = 20; // Amount of damage taken per collision
    public AudioClip playerHitSound; // "Aah" sound clip

    private AudioSource audioSource; // Audio source for playing the "aah" sound

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Add an AudioSource component to the player if not already present
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false; // Prevent the sound from playing on spawn
        audioSource.spatialBlend = 0f;   // Set to 2D sound
        audioSource.volume = 1f;        // Ensure the volume is not muted

        
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        // Play the "aah" sound when damage is taken
        if (playerHitSound != null)
        {
            audioSource.PlayOneShot(playerHitSound);
            Debug.Log("Aah sound played for damage!");
        }
        else
        {
            Debug.LogWarning("No 'playerHitSound' AudioClip assigned.");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // Add death handling logic here
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")) // Check if the collided object is tagged as 'Rock'
        {
            TakeDamage(damageFromRock);
            Debug.Log("Player hit by a rock! Health decreased.");
        }
    }
}

