using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LanguageSelection : MonoBehaviour
{
    public TextMeshProUGUI EnglishOption; // Assign English text UI element
    public TextMeshProUGUI HindiOption;   // Assign Hindi text UI element

    private void Awake()
    {
        string savedLanguage = PlayerPrefs.GetString("Language", "English");
        Debug.Log("Language loaded in Awake(): " + savedLanguage);

        SetLanguage(savedLanguage, false); // Ensure UI updates correctly, but don’t play sound
    }

    public void SetLanguage(string language, bool playSound = true)
    {
        Debug.Log("Setting language to: " + language);
        

        // Update UI based on language
        if (language == "Hindi")
        {
            EnglishOption.fontStyle = FontStyles.Normal;
            HindiOption.fontStyle = FontStyles.Bold;
            EnglishOption.color = Color.gray;
            HindiOption.color = Color.white;
        }
        else
        {
            EnglishOption.fontStyle = FontStyles.Bold;
            HindiOption.fontStyle = FontStyles.Normal;
            EnglishOption.color = Color.white;
            HindiOption.color = Color.gray;
        }

        // Save the language preference
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.Save();
        Debug.Log("Language saved: " + PlayerPrefs.GetString("Language"));

        // Play button click sound only if triggered by user action
        if (playSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
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