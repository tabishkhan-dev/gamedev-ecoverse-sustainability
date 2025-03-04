using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayManager : MonoBehaviour
{
    private bool isReturningToMenu = false; // Prevents multiple calls
    private string savedLanguage;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isReturningToMenu)
        {
            isReturningToMenu = true;
            savedLanguage = PlayerPrefs.GetString("Language", "English");
            Debug.Log("Loaded language in Start(): " + savedLanguage);
            ReturnToMenu();
        }
    }

    public void ReturnToMenu()
    {
        Debug.Log("Returning to Game Menu...");

        savedLanguage = PlayerPrefs.GetString("Language", "English");
        Debug.Log("Loaded language in Start(): " + savedLanguage);

        SaveVolumeSettings();

        // Delay the sound slightly to avoid double playing
        Invoke(nameof(PlayButtonClickSound), 0.05f);

        SceneManager.LoadScene("Game menu");
    }

    private void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("GameVolume", AudioListener.volume);
        PlayerPrefs.SetInt("StoryVolume", PlayerPrefs.GetInt("StoryVolume", 0));
        PlayerPrefs.Save();
    }
}