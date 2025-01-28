using UnityEngine;

public class ControlsPopup : MonoBehaviour
{
    public GameObject ControlsPanel; // Drag the ControlsPanel here

    public void ShowControls()
    {
        ControlsPanel.SetActive(true); // Show the panel
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }

    public void HideControls()
    {
        ControlsPanel.SetActive(false); // Hide the panel
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
    }
}
