using System.Collections; // Required for IEnumerator
using UnityEngine;

public class StreamBehavior : MonoBehaviour
{
    public int drowningDamage = 5;       // Amount of health decrease per second
    public float drowningInterval = 1f; // Time interval between health decreases

    private bool isDrowning = false;     // To track if the player is in the stream
    private HealthManager playerHealthManager; // Reference to the player's HealthManager
    private Coroutine drowningCoroutine; // Coroutine to handle drowning damage over time

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the stream is the player
        if (other.CompareTag("Player"))
        {
            playerHealthManager = other.GetComponent<HealthManager>();
            if (playerHealthManager != null)
            {
                isDrowning = true;
                drowningCoroutine = StartCoroutine(DrownPlayer());
                Debug.Log("Player started drowning in the stream.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop drowning when the player leaves the stream
        if (other.CompareTag("Player") && isDrowning)
        {
            isDrowning = false;
            if (drowningCoroutine != null)
            {
                StopCoroutine(drowningCoroutine);
                drowningCoroutine = null;
            }
            Debug.Log("Player exited the stream and stopped drowning.");
        }
    }

    private IEnumerator DrownPlayer()
    {
        while (isDrowning)
        {
            yield return new WaitForSeconds(drowningInterval); // Wait for the interval duration
            if (playerHealthManager != null)
            {
                playerHealthManager.TakeDamage(drowningDamage); // Decrease health
                Debug.Log("Player is drowning! Health decreased by " + drowningDamage);
            }
        }
    }
}
