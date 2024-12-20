using UnityEngine;
using TMPro;

public class ShieldManager : MonoBehaviour
{
    public GameObject shield; // Reference to the shield GameObject
    public TMP_Text shieldCounterText; // UI element to display the shield count
    private int shieldsAvailable; // Number of shields available
    private bool isShieldActive = false;

    private float shieldDuration = 10f; // Duration for which the shield is active
    private float shieldTimer = 0f;

    void Start()
    {
        // Retrieve the number of shields collected in the QuizLevel
        shieldsAvailable = PlayerPrefs.GetInt("ShieldCount", 0);
        Debug.Log("Shields available: " + shieldsAvailable);

        // Ensure the shield is inactive initially
        shield.SetActive(false);

        // Display the shield count on the UI
        UpdateShieldCounterUI();
    }

    void Update()
    {
        // Activate the shield if S key is pressed and shields are available
        if (Input.GetKeyDown(KeyCode.S) && shieldsAvailable > 0 && !isShieldActive)
        {
            ActivateShield();
        }

        // Handle the shield timer
        if (isShieldActive)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0)
            {
                DeactivateShield();
            }
        }
    }

    void ActivateShield()
    {
        if (shieldsAvailable <= 0)
        {
            Debug.LogWarning("No shields available!");
            return;
        }

        isShieldActive = true;
        shieldsAvailable--;
        shieldTimer = shieldDuration;

        // Save the updated shield count
        PlayerPrefs.SetInt("ShieldCount", shieldsAvailable);
        PlayerPrefs.Save();

        // Enable the shield GameObject
        shield.SetActive(true);
        UpdateShieldCounterUI();
        Debug.Log("Shield activated. Shields remaining: " + shieldsAvailable);
    }

    void DeactivateShield()
    {
        isShieldActive = false;

        // Disable the shield GameObject
        shield.SetActive(false);
        Debug.Log("Shield deactivated.");
    }

    void UpdateShieldCounterUI()
    {
        // Update the UI text with the remaining shields
        if (shieldCounterText != null)
        {
            shieldCounterText.text = $"Shields Collected: {shieldsAvailable}";
        }
    }
}

