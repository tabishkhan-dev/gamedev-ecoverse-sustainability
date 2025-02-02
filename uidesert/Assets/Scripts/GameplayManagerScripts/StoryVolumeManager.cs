using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StoryVolumeManager : MonoBehaviour
{
    public static readonly string StoryVolumeKey = "StoryVolume"; // Make this public static
    public Button storyVolumeButton;
    public Sprite muteIcon;
    public Sprite unmuteIcon;
    private Image buttonImage;

    void Start()
    {
        // Get the Image component from the button
        buttonImage = storyVolumeButton.GetComponent<Image>();

        // Load saved volume state (default: unmuted)
        bool isMuted = PlayerPrefs.GetInt(StoryVolumeKey, 0) == 1;
        UpdateStoryVolume(isMuted);

        // Add listener to toggle sound on button click
        storyVolumeButton.onClick.AddListener(ToggleStoryVolume);
    }

    void ToggleStoryVolume()
    {
        bool isMuted = PlayerPrefs.GetInt(StoryVolumeKey, 0) == 1;
        isMuted = !isMuted; // Toggle state

        // Save new state
        PlayerPrefs.SetInt(StoryVolumeKey, isMuted ? 1 : 0);
        PlayerPrefs.Save();

        // Update UI and audio settings
        UpdateStoryVolume(isMuted);
    }

    void UpdateStoryVolume(bool isMuted)
{
    // Change button image based on mute state
    if (buttonImage != null)
    {
        buttonImage.sprite = isMuted ? muteIcon : unmuteIcon;
    }

    // Apply mute/unmute to all VideoPlayers in the scene
    VideoPlayer[] videoPlayers = Object.FindObjectsByType<VideoPlayer>(FindObjectsSortMode.None);
    foreach (VideoPlayer vp in videoPlayers)
    {
        vp.SetDirectAudioMute(0, isMuted);
    }
}

}
