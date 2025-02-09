using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton instance

    public GameObject aboutPanel; // Drag the AboutPanel GameObject here in the Inspector
    public AudioSource audioSource; // Drag the Audio Source Component here in the Inspector
    public AudioClip buttonClickSound; // Assign the button click sound in the Inspector

    private void Awake()
    {
        // Singleton setup: ensure only one instance of AudioManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    public void ShowAbout()
    {
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(true); // Activate the AboutPanel
            PlayAudio(); // Play the background audio
        }
    }

    public void HideAbout()
    {
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(false); // Deactivate the AboutPanel
            StopAudio(); // Stop the background audio
        }
    }

    private void PlayAudio()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play(); // Start playing the audio
        }
    }

    private void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop(); // Stop the audio
        }
    }

    // Method to play button click sounds
    public void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound); // Play the click sound
        }
    }

    // Debugging helper: Play sound when a button click happens
    public void OnButtonClick()
    {
        PlayButtonClickSound();
    }
}
