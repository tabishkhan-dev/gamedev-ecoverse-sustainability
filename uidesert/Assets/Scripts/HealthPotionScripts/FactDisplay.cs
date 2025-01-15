using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TubeCollectionManager : MonoBehaviour
{
    public GameObject popup; // Reference to the popup UI GameObject
    public TextMeshProUGUI popupText; // Reference to the TMP Text for the knowledge fact
    public int healthValue = 20; // Amount of health restored per tube

    private List<string> remainingFacts; // List to keep track of unused facts

    private string[] knowledgeFacts = new string[]
    {
        "Plant trees as one tree absorbs\n 22 kgs of CO2 each year,\n helping fight climate change.",
        "Reduce waste to help protect\n marine animals as over\n 1 million die each year due to plastic.",
        "When you think green, act green:\n turning off unused appliances\n can save up to 30% \n of your electricity annually.",
        "Renewable energy from the Sun\n can meet Earth's annual energy needs\n with just 1.5 hours \n of sunlight.",
        "Driving an electric car produces\n 50% fewer greenhouse gas \n emissions over its lifetime.",
        "Save water as global water demand is projected \n to increase by 55% by 2050.",
        "Water is life as only <1% of Earth's water is \n accessible for human use, \n making it a precious resource.",
        "46 percent is the recycling rate\n for plastic waste in Germany.",
        "Rainwater harvesting can collect up to 600 gallons \n (2,271 liters) from just one inch of rain \n on a 1,000 sq. ft roof.",
        "Protect wildlife as illegal wildlife trade is $23 billion \n annually, causing extinction of species."
    };

    void Start()
    {
        // Initialize the list of remaining facts with all the facts
        remainingFacts = new List<string>(knowledgeFacts);

        // Ensure the popup is initially disabled
        popup.SetActive(false);
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
                HealthManager healthManager = player.GetComponent<HealthManager>();
                if (healthManager != null)
                {
                    // Restore the player's health
                    healthManager.AddHealth(healthValue);
                }
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
            string randomFact = remainingFacts[randomIndex];

            // Display the fact
            popupText.text = randomFact;

            // Remove the fact from the list so it won't be reused
            remainingFacts.RemoveAt(randomIndex);

            // Enable the popup
            popup.SetActive(true);

            // Hide the popup after 3 seconds
            Invoke(nameof(HidePopup), 5f);
        }
        else
        {
            // If no facts are remaining, display a placeholder message (optional)
            popupText.text = "No more facts available!";
            popup.SetActive(true);

            // Hide the popup after 3 seconds
            Invoke(nameof(HidePopup), 3f);
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
