using UnityEngine;
using TMPro;

public class LanguageSelection : MonoBehaviour
{
    public TextMeshProUGUI EnglishOption; // Drag your English Text object here
    public TextMeshProUGUI GermanOption;  // Drag your German Text object here

    private void Start()
    {
          //PlayerPrefs.DeleteAll();
    //Debug.Log("PlayerPrefs has been cleared!");
        // Initialize language from saved preferences, default to English if none is set
        string savedLanguage = PlayerPrefs.GetString("Language", "English");
        SetLanguage(savedLanguage);
    }

    public void SetLanguage(string language)
    {
        Debug.Log("Setting language to: " + language);
        if (language == "English")
        {
            EnglishOption.color = Color.gray; // Dim English
            GermanOption.color = Color.white; // Keep German white
        }
        else if (language == "German")
        {
            EnglishOption.color = Color.white; // Keep English white
            GermanOption.color = Color.gray;  // Dim German
        }

        // Save the selected language in PlayerPrefs
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.Save();
    }

    public void SelectEnglish()
    {
        SetLanguage("English");
    }

    public void SelectGerman()
    {
        Debug.Log("German button clicked");
        SetLanguage("German");
    }
    
}




