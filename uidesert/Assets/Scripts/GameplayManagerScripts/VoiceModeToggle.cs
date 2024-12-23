using UnityEngine;
using UnityEngine.UI;

public class VoiceModeToggle : MonoBehaviour
{
    public Toggle VoiceToggle; // Drag your Toggle object here

    private void Start()
    {
        // Load saved preference for Voice Mode; default to enabled (true)
        bool isVoiceModeEnabled = PlayerPrefs.GetInt("VoiceMode", 1) == 1;
        VoiceToggle.isOn = isVoiceModeEnabled;

        // Add listener to handle changes when the toggle is clicked
        VoiceToggle.onValueChanged.AddListener(OnVoiceToggleChanged);
    }

    private void OnVoiceToggleChanged(bool isOn)
    {
        // Save the updated preference
        PlayerPrefs.SetInt("VoiceMode", isOn ? 1 : 0);
        PlayerPrefs.Save();

        // Log for debugging
        Debug.Log("Voice Mode is now " + (isOn ? "Enabled" : "Disabled"));

        // Implement additional functionality here, if needed
        if (isOn)
        {
            EnableVoiceMode();
        }
        else
        {
            DisableVoiceMode();
        }
    }

    private void EnableVoiceMode()
    {
        // Add logic for enabling voice mode (e.g., start voice recognition)
        Debug.Log("Voice Mode Enabled");
    }

    private void DisableVoiceMode()
    {
        // Add logic for disabling voice mode (e.g., stop voice recognition)
        Debug.Log("Voice Mode Disabled");
    }
}
