using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TubeCollectionManagerl2 : MonoBehaviour
{
    public GameObject popup; // Reference to the popup UI GameObject
    public TextMeshProUGUI popupText; // Reference to the TMP Text for the knowledge fact
    public int healthValue = 20; // Amount of health restored per tube
    private List<string> remainingFacts; // List to keep track of unused facts

    public AudioClip tubecollectionSound;
    private AudioSource audioSource;

    private string[] knowledgeFacts = new string[]
    {
        "Mindful Consumption \n of Energy.",
        "Repurposing water to \n irrigate plants.",
        "Eliminate Single-Use Plastics.",
        "Make Sustainable \n Food Choices."
    };

    void Start()
    {
        // Initialize the list of remaining facts with all the facts
        remainingFacts = new List<string>(knowledgeFacts);

        // Ensure the popup is initially disabled
        popup.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has the tag "Tube"
        if (other.CompareTag("Tube"))
        {
            // Find the player object in the scene
            GameObject player = GameObject.FindWithTag("Player");

            if (player != null)
            {
                // Get the HealthManager component from the player
                HealthManagerLevel2 healthManager = player.GetComponent<HealthManagerLevel2>();
                if (healthManager != null)
                {
                    // Restore the player's health
                    healthManager.AddHealth(healthValue);
                }
            }
            // Play collection sound
            if (tubecollectionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(tubecollectionSound);
            }

            // Destroy the tube
            Destroy(other.gameObject);

            // Display a random fact in the popup
            ShowRandomFact();
        }
    }

    void ShowRandomFact()
    {
        if (remainingFacts.Count > 0)
        {
            // Select a random fact from the remaining facts
            int randomIndex = Random.Range(0, remainingFacts.Count);
            string selectedFact = remainingFacts[randomIndex];

            // Display the fact
            popupText.text = selectedFact;

            // Remove the displayed fact from the list
            remainingFacts.RemoveAt(randomIndex);

            // Enable the popup
            popup.SetActive(true);

            // Hide the popup after 6 seconds
            Invoke(nameof(HidePopup), 6f);
        }
        else
        {
            popupText.text = "No more facts available!";
            popup.SetActive(true);
            Invoke(nameof(HidePopup), 6f);
        }
    }

    void HidePopup()
    {
        // Disable the popup
        popup.SetActive(false);

        // Clear the popup text
        popupText.text = string.Empty;
    }
}
