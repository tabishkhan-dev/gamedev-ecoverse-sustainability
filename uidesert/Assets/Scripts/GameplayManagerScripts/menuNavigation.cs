using UnityEngine;

public class MenuNavigationManager : MonoBehaviour
{
    public GameObject mainMenu;      // Drag the MainMenu GameObject here in the Inspector
    public GameObject settingsMenu; // Drag the SettingsMenu GameObject here in the Inspector

    // Method to show the Settings Menu
    public void ShowSettingsMenu()
    {
        if (mainMenu != null)
            mainMenu.SetActive(false); // Hide MainMenu

        if (settingsMenu != null)
            settingsMenu.SetActive(true); // Show SettingsMenu
    }

    // Method to return to the Main Menu
    public void ShowMainMenu()
    {
        if (settingsMenu != null)
            settingsMenu.SetActive(false); // Hide SettingsMenu

        if (mainMenu != null)
            mainMenu.SetActive(true); // Show MainMenu
    }
}
