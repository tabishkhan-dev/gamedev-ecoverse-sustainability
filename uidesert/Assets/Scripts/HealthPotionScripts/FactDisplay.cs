using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TubeCollectionManager : MonoBehaviour
{
    public GameObject popup; // Reference to the popup UI GameObject
    public TextMeshProUGUI popupText; // Reference to the TMP Text for the knowledge fact
    public int healthValue = 20; // Amount of health restored per tube
    private List<(string key, string fact)> remainingFacts; // List to keep track of unused facts with keys
    private List<string> selectedKeys = new List<string>(); // List to store selected fact keys

    private (string key, string fact)[] knowledgeFactsWithKeys = new (string key, string fact)[]
    {
        ("fact1", "Plant trees as one tree absorbs\n 22 kgs of CO2 each year,\n helping fight climate change."),
        ("fact2", "Reduce waste to help protect\n marine animals as over\n 1 million die each year due to plastic."),
        ("fact3", "When you think green, act green: turning off unused appliances can save up to 30% \n of your electricity annually."),
        ("fact4", "Renewable energy from the Sun\n can meet Earth's annual energy needs\n with just 1.5 hours \n of sunlight."),
        ("fact5", "Driving an electric car produces\n 50% fewer greenhouse gas \n emissions over its lifetime."),
        ("fact6", "Save water as global water demand is projected \n to increase by 55% by 2050."),
        ("fact7", "Water is life as only <1% of Earth's water is \n accessible for human use, \n making it a precious resource."),
        ("fact8", "46 percent is the recycling rate\n for plastic waste in Germany."),
        ("fact9", "Rainwater harvesting can collect up to 600 gallons (2,271 liters) from just one inch of rain on a 1,000 sq. ft roof."),
        ("fact10", "Protect wildlife as illegal wildlife trade is $23 billion \n annually, causing extinction of species."),
        ("fact11", "The carbon footprint of deforestation is 10-15% of global emissions,\n decreasing Earth’s ability to absorb CO2")
    };

    void Start()
    {
        PlayerPrefs.DeleteAll(); // Clear all previous data
        PlayerPrefs.SetInt("FactCount", 0); // Reset FactCount to 0

        // Initialize the list of remaining facts with all the facts
        remainingFacts = new List<(string key, string fact)>(knowledgeFactsWithKeys);

        PreSelectFacts();

        // Ensure the popup is initially disabled
        popup.SetActive(false);
    }

    private void PreSelectFacts()
    {
        for (int i = 0; i < Mathf.Min(4, remainingFacts.Count); i++)
        {
            int randomIndex = Random.Range(0, remainingFacts.Count);

            // Add the selected key to the selectedKeys list
            selectedKeys.Add(remainingFacts[randomIndex].key);

            // Remove the selected fact from remaining facts
            remainingFacts.RemoveAt(randomIndex);
        }
        Debug.Log("Selected Keys: " + string.Join(", ", selectedKeys));

        // Store the selected keys to pass to Level 2
        StoreSelectedKeys();
    }

    private void StoreSelectedKeys()
    {
        // Convert selected keys list to a comma-separated string
        string keysString = string.Join(",", selectedKeys);

        // Save the keys to PlayerPrefs for the next level
        PlayerPrefs.SetString("SelectedFactKeys", keysString);
        PlayerPrefs.Save();
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
            ShowFact();
        }
    }

    void ShowFact()
    {
        if (selectedKeys.Count > 0)
        {
            // Get the first key from the selected keys
            string selectedKey = selectedKeys[0];

            // Find the fact directly from the full knowledgeFactsWithKeys array
            var factEntry = System.Array.Find(knowledgeFactsWithKeys, f => f.key == selectedKey);

            if (factEntry != default)
            {
                // Display the fact
                popupText.text = factEntry.fact;

                // Remove the displayed key from selectedKeys
                selectedKeys.RemoveAt(0);

                // Enable the popup
                popup.SetActive(true);

                // Hide the popup after 3 seconds
                Invoke(nameof(HidePopup), 5f);
            }
            else
            {
                Debug.LogError($"Fact with key {selectedKey} not found in knowledgeFactsWithKeys!");
            }
        }
        else
        {
            popupText.text = "No more facts available!";
            popup.SetActive(true);
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
