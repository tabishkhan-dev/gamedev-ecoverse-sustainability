using UnityEngine;
using UnityEngine.UI;

public class TimerIcon : MonoBehaviour
{
    public Image timerImage; // Assign the UI Image in the Inspector
    public float totalTime = 30f; // Timer duration in seconds
    private float currentTime;

    void Start()
    {
        currentTime = totalTime; // Initialize timer
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime; // Decrease time
            timerImage.fillAmount = currentTime / totalTime; // Update UI Fill
        }
        else
        {
            timerImage.fillAmount = 0; // Ensure fill is empty
            enabled = false; // Stop the update loop
        }
    }

    // **New function to reset and restart the timer**
    public void RestartTimer()
    {
        currentTime = totalTime;  // Reset timer
        timerImage.fillAmount = 1; // Reset UI Fill to full
        enabled = true; // Reactivate update loop
    }
}
