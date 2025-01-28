using UnityEngine;
using TMPro;

public class LanguageSelection : MonoBehaviour
{
    public TextMeshProUGUI EnglishOption; // Drag your English Text object here
    public TextMeshProUGUI HindiOption;  // Drag your Hindi Text object here

    private void Start()
    {
        // Initialize language from saved preferences, default to English if none is set
        string savedLanguage = PlayerPrefs.GetString("Language", "English");
        
        // Set initial language and apply formatting
        if (string.IsNullOrEmpty(savedLanguage))
        {
            savedLanguage = "English"; // Default language
            PlayerPrefs.SetString("Language", savedLanguage);
            PlayerPrefs.Save();
        }

        SetLanguage(savedLanguage);
    }

    public void SetLanguage(string language)
    {
        Debug.Log("Setting language to: " + language);
        if (language == "Hindi")
        {
            // Apply bold effect to Hindi and dim English
            EnglishOption.fontStyle = FontStyles.Normal; // Remove bold from English
            HindiOption.fontStyle = FontStyles.Bold;    // Make Hindi bold

            EnglishOption.color = Color.gray; // Dim English
            HindiOption.color = Color.white; // Keep Hindi white
            if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        }
        else if (language == "English")
        {
            // Apply bold effect to English and dim Hindi
            EnglishOption.fontStyle = FontStyles.Bold;    // Make English bold
            HindiOption.fontStyle = FontStyles.Normal;    // Remove bold from Hindi

            EnglishOption.color = Color.white; // Keep English white
            HindiOption.color = Color.gray;   // Dim Hindi
            if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        }

        // Save the selected language in PlayerPrefs
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.Save();
    }

    public void SelectEnglish()
    {
        SetLanguage("English");
    }

    public void SelectHindi()
    {
        SetLanguage("Hindi");
    }
}
