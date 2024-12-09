using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro support

public class SolarPanelCollector : MonoBehaviour
{
    public int totalSolarPanels = 3; // Total number of solar panels to collect
    private int collectedPanels = 0; // Number of panels collected

    public TMP_Text collectionMessage; // Text to display the collection message
    public Image fuelMeter; // The fuel meter UI image
    public float fuelIncreasePerPanel = 0.33f; // Fuel bar increment per panel

    public AudioClip collectionSound; // Sound to play when a panel is collected
    private AudioSource audioSource; // Reference to the AudioSource

    private Collider panelToCollect = null; // Tracks the panel the player can collect

    private float currentFuel = 0f; // Tracks the current fuel level

    public GameObject alignmentTask; // Task object to align solar panels (UI or interactive object)

    void Start()
    {
        // Initialize the collection message and fuel meter
        collectionMessage.text = $"0 of {totalSolarPanels} collected";
        fuelMeter.fillAmount = 0f; // Start with an empty fuel meter
        alignmentTask.SetActive(false); // Hide alignment task initially

        // Get or Add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

         // Disable the AudioSource component during gameplay
    
    }

    void Update()
    {
        // Check if the player is near a solar panel and presses the "E" key
        if (panelToCollect != null && Input.GetKeyDown(KeyCode.E))
        {
            CollectSolarPanel(panelToCollect);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object is tagged as a solar panel
        if (other.CompareTag("SolarPanel"))
        {
            panelToCollect = other; // Store the reference to the panel
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Clear the reference when leaving the solar panel
        if (other.CompareTag("SolarPanel"))
        {
            panelToCollect = null;
        }
    }

    private void CollectSolarPanel(Collider panel)
    {
        collectedPanels++; // Increment the collected panels count

        // Play the collection sound
        if (collectionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectionSound);
        }

        // Update the collection message
        collectionMessage.text = $"{collectedPanels} of {totalSolarPanels} collected";

        // Update the fuel meter
        currentFuel += fuelIncreasePerPanel;
        UpdateFuelMeter(currentFuel);

        // Destroy the collected solar panel
        Destroy(panel.gameObject);

        // Check if all solar panels are collected
        if (collectedPanels == totalSolarPanels)
        {
            // Trigger the alignment task
            TriggerAlignmentTask();
        }
    }

    private void UpdateFuelMeter(float fuelValue)
    {
        // Update the fuel meter UI and clamp it between 0 and 1
        fuelMeter.fillAmount = Mathf.Clamp01(fuelValue);
    }

    private void TriggerAlignmentTask()
    {
        Debug.Log("All solar panels collected! Prepare to align them towards the sun.");

        // Display or activate the alignment task
        alignmentTask.SetActive(true);

        // Update the collection message for alignment
        collectionMessage.text = "Align the solar panels to the sun!";
    }
}
