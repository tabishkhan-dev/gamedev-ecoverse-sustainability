using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject aboutPanel; // Drag the AboutPanel GameObject here in the Inspector
    public AudioSource audioSource; // Drag the Audio Source Component here in the Inspector

    public void ShowAbout()
    {
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(true); // Activate the AboutPanel
            PlayAudio(); // Play the audio
        }
    }

    public void HideAbout()
    {
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(false); // Deactivate the AboutPanel
            StopAudio(); // Stop the audio
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
}

