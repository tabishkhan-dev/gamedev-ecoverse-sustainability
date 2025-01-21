using UnityEngine;
using UnityEngine.UI;

public class StoryModeManager : MonoBehaviour
{
    public Toggle StoryModeToggle; // Assign your StoryModeToggle in the Inspector

    private void Start()
    {
        // Load saved story mode preference
        bool isStoryModeOn = PlayerPrefs.GetInt("StoryMode", 1) == 1; // Default is On
        StoryModeToggle.isOn = isStoryModeOn;

        // Add listener for toggle changes
        StoryModeToggle.onValueChanged.AddListener(OnStoryModeToggleChanged);
    }

    private void OnStoryModeToggleChanged(bool isOn)
    {
        Debug.Log("Story Mode: " + (isOn ? "On" : "Off"));
        // Save the new state
        PlayerPrefs.SetInt("StoryMode", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}

