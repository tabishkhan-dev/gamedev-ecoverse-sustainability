using UnityEngine;
using UnityEngine.UI;

public class VoiceModeToggle : MonoBehaviour
{
    public Toggle myToggle;
    private const string TOGGLE_KEY = "VoiceModeState"; // Unique key for PlayerPrefs

    private void Start()
    {
        // Load the saved state (default 1 = on if not set)
        bool savedState = PlayerPrefs.GetInt(TOGGLE_KEY, 1) == 1;
        myToggle.isOn = savedState;

        // Listen for toggle changes
        myToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        // Save the new state (1 for true, 0 for false)
        PlayerPrefs.SetInt(TOGGLE_KEY, isOn ? 1 : 0);
        PlayerPrefs.Save();

        // Print messages based on toggle state
        if (isOn)
        {
            Debug.Log("mode enable");
        }
        else
        {
            Debug.Log("mode disable");
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }



}
