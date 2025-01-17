using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindTurbineCollector : MonoBehaviour
{
    public int totalWindTurbines = 4; // Total number of wind turbines to collect
    private int collectedTurbines = 0; // Number of collected turbines

    public TMP_Text turbineCollectionMessage; // UI Text for turbine collection progress
    public Image fuelMeter; // UI Image for the fuel bar
    public float fuelIncreasePerTurbine = 0.25f; // Fuel bar increment per turbine (4 turbines -> 0.25 per turbine)

    public AudioClip collectionSound; // Sound to play on turbine collection

    private AudioSource audioSource;
    private float currentFuel = 0f; // Tracks current fuel level for wind turbines

    void Start()
    {
        // Initialize UI and fuel bar
        fuelMeter.fillAmount = currentFuel;
        UpdateTurbineCollectionMessage();

        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider is tagged as a WindTurbine
        if (other.CompareTag("WindTurbine"))
        {
            CollectWindTurbine(other);
        }
    }

    private void CollectWindTurbine(Collider turbine)
    {
        collectedTurbines++; // Increment collected turbines count

        // Update the UI to reflect the current count
        UpdateTurbineCollectionMessage();

        // Play collection sound
        if (collectionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectionSound);
        }

        // Update the fuel bar
        currentFuel += fuelIncreasePerTurbine;
        UpdateFuelMeter(currentFuel);

        // Destroy the collected turbine
        Destroy(turbine.gameObject);
    }

    private void UpdateTurbineCollectionMessage()
    {
        turbineCollectionMessage.text = $"{collectedTurbines} of {totalWindTurbines} wind turbines collected";
    }

    private void UpdateFuelMeter(float fuelValue)
    {
        fuelMeter.fillAmount = Mathf.Clamp01(fuelValue);
    }
}
