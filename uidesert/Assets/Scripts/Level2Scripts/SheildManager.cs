using UnityEngine;
using TMPro;

public class ShieldManager : MonoBehaviour
{
    public GameObject shield; // Reference to the shield GameObject
    public TMP_Text shieldCounterText; // UI element to display the shield count
    private int shieldsAvailable; // Number of shields available
    private bool isShieldActive = false;

    private float shieldDuration = 10f; // Duration for which the shield is active
    private float smokeDisableDuration = 7f; // Duration for which smoke effects are disabled
    private float shieldTimer = 0f;

    private GameObject[] icebergs; // Array of iceberg GameObjects tagged as "ground"
    private GameObject[] smokeEffects; // Array of smoke particle systems tagged as "smoke"

    void Start()
    {
        // Retrieve the number of shields collected in the QuizLevel
        shieldsAvailable = PlayerPrefs.GetInt("ShieldCount", 0);
        Debug.Log("Shields available: " + shieldsAvailable);

        // Ensure the shield is inactive initially
        shield.SetActive(false);

        // Display the shield count on the UI
        UpdateShieldCounterUI();

        // Find objects by tag
        InitializeReferences();
    }

    void InitializeReferences()
    {
        // Find all icebergs tagged as "ground"
        icebergs = GameObject.FindGameObjectsWithTag("Ground");
        if (icebergs == null || icebergs.Length == 0)
        {
            Debug.LogWarning("No icebergs tagged as 'Ground' found in the scene.");
        }
        else
        {
            Debug.Log($"Found {icebergs.Length} icebergs tagged as 'Ground' in the scene.");
        }

        // Find all smoke particle systems tagged as "smoke"
        smokeEffects = GameObject.FindGameObjectsWithTag("smoke");
        if (smokeEffects == null || smokeEffects.Length == 0)
        {
            Debug.LogWarning("No smoke particle systems tagged as 'smoke' found in the scene.");
        }
        else
        {
            Debug.Log($"Found {smokeEffects.Length} smoke effects tagged as 'smoke' in the scene.");
        }
    }

    void Update()
    {
        // Activate the shield if S key is pressed and shields are available
        if (Input.GetKeyDown(KeyCode.Z) && shieldsAvailable > 0 && !isShieldActive)
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

        // Pause iceberg melting
        foreach (var iceberg in icebergs)
        {
            if (iceberg != null)
            {
                IcebergBehavior icebergBehavior = iceberg.GetComponent<IcebergBehavior>();
                if (icebergBehavior != null)
                {
                    icebergBehavior.PauseMelting(10f); // Pause melting for 10 seconds
                }
            }
        }

        // Immediately stop smoke effects
        foreach (var smoke in smokeEffects)
        {
            if (smoke != null)
            {
                ParticleSystem particleSystem = smoke.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // Stop immediately
                    StartCoroutine(EnableSmokeEffectAfterDelay(particleSystem, smokeDisableDuration));
                }
            }
        }
    }

    void DeactivateShield()
    {
        isShieldActive = false;

        // Disable the shield GameObject
        shield.SetActive(false);
        Debug.Log("Shield deactivated.");
    }

    System.Collections.IEnumerator EnableSmokeEffectAfterDelay(ParticleSystem particleSystem, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    void UpdateShieldCounterUI()
    {
        if (shieldCounterText != null)
        { 
            if (shieldsAvailable > 0)
            {
                shieldCounterText.text = $"{shieldsAvailable}/3";
                 // Set to default color (or your preferred color)
            }
            else
            {
                shieldCounterText.text = "No Shields!";
                shieldCounterText.color = Color.red; // Change text color to red
            }
        }
    }
}