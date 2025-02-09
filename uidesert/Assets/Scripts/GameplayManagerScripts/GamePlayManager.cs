using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMenu();
        }
    }

    public void ReturnToMenu()
    {
        Debug.Log("Returning to Game Menu...");
        
        // Save settings before switching scenes to ensure they're not reset
        SaveVolumeSettings();

        SceneManager.LoadScene("Game menu"); // Ensure this matches your Game menu scene name

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }

    // This method saves the volume settings (both game and story)
    private void SaveVolumeSettings()
    {
        // Example for saving game volume
        PlayerPrefs.SetFloat("GameVolume", AudioListener.volume);
        
        // Example for saving story volume state (mute or unmute)
        bool isMuted = PlayerPrefs.GetInt("StoryVolume", 0) == 1;
        PlayerPrefs.SetInt("StoryVolume", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
}
