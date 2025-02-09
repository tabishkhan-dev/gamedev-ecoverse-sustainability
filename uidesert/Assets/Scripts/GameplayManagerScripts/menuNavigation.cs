using UnityEngine;

public class MenuNavigationManager : MonoBehaviour
{
    public GameObject mainMenu;      // Assign MainMenu in Inspector
    public GameObject settingsMenu; // Assign SettingsMenu in Inspector

    // Show Settings Menu
    public void ShowSettingsMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        Debug.Log("Switching to Settings Menu");
        Debug.Log("MainMenu: " + (mainMenu != null && mainMenu.activeSelf));
        Debug.Log("SettingsMenu: " + (settingsMenu != null && settingsMenu.activeSelf));
        
        if (mainMenu != null) mainMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(true);
    }

    // Show Main Menu
    public void ShowMainMenu()
    {
        Debug.Log("Returning to Main Menu");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }

        // Hide SettingsMenu and Show MainMenu
        if (settingsMenu != null) settingsMenu.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(true);
        else Debug.LogError("MainMenu is not assigned!");
    }
}

