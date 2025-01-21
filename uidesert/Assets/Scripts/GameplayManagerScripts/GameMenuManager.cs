using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene loading

public class GameMenuManager : MonoBehaviour
{
    public GameObject mainMenu;    // Drag the MainMenu GameObject here in the Inspector
    public GameObject aboutPanel; // Drag the AboutPanel GameObject here in the Inspector

    // Method to reset progress
    private void ResetProgress()
    {
        PlayerPrefs.DeleteKey("CollectedSolarPanels"); // Reset solar panel progress
        PlayerPrefs.DeleteKey("CollectedTurbines");   // Reset wind turbine progress
        PlayerPrefs.DeleteKey("CurrentFuel");         // Reset fuel meter
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.Save();                           // Save the reset state
    }

    // Method to start the game
    public void StartGame()
    {
        ResetProgress(); // Reset progress before starting the game
        SceneManager.LoadScene("SampleScene"); // Make sure the scene name matches exactly
    }

    // Show About Panel
    public void ShowAbout()
    {
        if (aboutPanel != null && mainMenu != null)
        {
            aboutPanel.SetActive(true);  // Show AboutPanel
            mainMenu.SetActive(false);  // Hide MainMenu

            // Start scrolling the text
            AutoScrollText scrollScript = aboutPanel.GetComponentInChildren<AutoScrollText>();
            if (scrollScript != null)
            {
                scrollScript.StartScrolling();
            }
        }
    }

    // Hide About Panel
    public void HideAbout()
    {
        Debug.Log("HideAbout function called!");
        if (aboutPanel != null && mainMenu != null)
        {
            aboutPanel.SetActive(false); // Hide AboutPanel
            mainMenu.SetActive(true);   // Show MainMenu
            Debug.Log("AboutPanel hidden, MainMenu shown.");
        }
        else
        {
            Debug.Log("References are missing!");
        }
    }

    public class AudioManager : MonoBehaviour
    {
        public AudioSource aboutAudio; // Drag the Audio Source here in the Inspector.

        public void PlayAboutAudio()
        {
            if (aboutAudio != null)
            {
                aboutAudio.Play();
            }
        }

        public void StopAboutAudio()
        {
            if (aboutAudio != null)
            {
                aboutAudio.Stop();
            }
        }
    }
}
