using UnityEngine;

public class AboutMenuNavigationManager : MonoBehaviour
{
    public GameObject mainMenu;      // Assign MainMenu in Inspector
    public GameObject AboutMenu; // Assign SettingsMenu in Inspector

    // Show Settings Menu
    public void ShowAboutMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        Debug.Log("Switching to Settings Menu");
        Debug.Log("MainMenu: " + (mainMenu != null && mainMenu.activeSelf));
        Debug.Log("AboutMenu: " + (AboutMenu != null && AboutMenu.activeSelf));
        
        if (mainMenu != null) mainMenu.SetActive(false);
        if (AboutMenu != null) AboutMenu.SetActive(true);
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
        if (AboutMenu != null) AboutMenu.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(true);
        else Debug.LogError("MainMenu is not assigned!");
    }
}
