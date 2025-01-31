using UnityEngine;

public class AudioPopupController : MonoBehaviour
{
    public GameObject audioPopupImage; // Assign your Audio Popup Image in the Inspector

    private bool isPopupActive = false; // Track popup state

    public void ToggleAudioPopup()
    {
        isPopupActive = !isPopupActive; // Toggle state
        audioPopupImage.SetActive(isPopupActive); // Show/Hide popup
    }

    public void CloseAudioPopup()
    {
        isPopupActive = false;
        audioPopupImage.SetActive(false); // Ensure it hides when Back is pressed
    }
}

