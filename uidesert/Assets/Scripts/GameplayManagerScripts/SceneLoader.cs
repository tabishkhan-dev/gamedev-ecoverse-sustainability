using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        // Play click sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }

        // Load the scene
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        // Play click sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }

        // Exit the game
        Application.Quit();
    }
}
