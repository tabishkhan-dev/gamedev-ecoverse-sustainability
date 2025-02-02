using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeManager : MonoBehaviour
{
    public Slider volumeSlider; // Reference to UI Slider
    public TMP_Text volumeValueText; // Optional: Displays numeric volume level

    private const string VolumeKey = "GameVolume"; // PlayerPrefs Key

    void Start()
    {
        // Load saved volume or set default
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;

        // Add listener for real-time volume change
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    void UpdateVolume(float volume)
    {
        AudioListener.volume = volume; // Adjust all game sounds
        PlayerPrefs.SetFloat(VolumeKey, volume); // Save volume setting
        PlayerPrefs.Save(); // Ensure it's saved permanently

        if (volumeValueText != null)
        {
            volumeValueText.text = $"Volume:"; // Display percentage
        }
    }
}
