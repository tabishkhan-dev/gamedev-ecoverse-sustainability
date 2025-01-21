using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene loading
using UnityEngine.Video;           // Required for video playback;

public class GameMenuManager : MonoBehaviour
{
    public string introVideoName = "IntroStory.mp4"; // Name of the video in StreamingAssets folder

    // A black overlay object for smooth transitions
    private GameObject blackOverlay;

    // Method to reset progress
    private void ResetProgress()
    {
        PlayerPrefs.DeleteKey("CollectedSolarPanels"); // Reset solar panel progress
        PlayerPrefs.DeleteKey("CollectedTurbines");   // Reset wind turbine progress
        PlayerPrefs.DeleteKey("CurrentFuel");         // Reset fuel meter
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.Save();                           // Save the reset state
    }

    // Method to start the game
    public void StartGame()
    {
        ResetProgress(); // Reset progress before starting the game

        // Show the black overlay for smooth transition
        ShowBlackOverlay();

        // Disable the main canvas (tagged as "mainCanvas")
        GameObject mainCanvas = GameObject.FindWithTag("mainCanvas");
        if (mainCanvas != null)
        {
            mainCanvas.SetActive(false);
        }

        // Play the intro video
        PlayIntroVideo(() =>
        {
            // Callback to load the next scene after the video ends
            SceneManager.LoadScene("SampleScene"); // Make sure the scene name matches exactly
        });
    }

    private void PlayIntroVideo(System.Action onVideoComplete)
    {
        Debug.Log("Initializing Intro video...");

        // Create a new GameObject for the VideoPlayer
        GameObject videoPlayerObject = new GameObject("IntroVideoPlayer");
        VideoPlayer videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();

        // Set the video path (ensure the video is in the StreamingAssets folder)
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, introVideoName);
        Debug.Log("Video path: " + videoPath);
        videoPlayer.url = videoPath;

        // Create a Render Texture for the video
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        renderTexture.Create();
        videoPlayer.targetTexture = renderTexture;

        // Create a new UI Image to display the Render Texture
        GameObject rawImageObject = new GameObject("IntroVideoRawImage");
        UnityEngine.UI.RawImage rawImage = rawImageObject.AddComponent<UnityEngine.UI.RawImage>();
        rawImage.texture = renderTexture;

        // Attach the RawImage to a full-screen Canvas
        GameObject canvasObject = new GameObject("IntroVideoCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>().uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        rawImageObject.transform.SetParent(canvas.transform, false);

        rawImage.rectTransform.anchorMin = Vector2.zero; // Bottom-left corner
        rawImage.rectTransform.anchorMax = Vector2.one;  // Top-right corner
        rawImage.rectTransform.offsetMin = Vector2.zero; // No offset
        rawImage.rectTransform.offsetMax = Vector2.zero; // No offset

        // Configure VideoPlayer
        videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
        videoPlayer.isLooping = false;

        // Event: When the video finishes
        videoPlayer.loopPointReached += (vp) =>
        {
            Debug.Log("Intro video finished playing.");

            // Keep the black overlay active during the transition to the next scene
            ShowBlackOverlay();

            onVideoComplete?.Invoke(); // Trigger the callback after the video ends

            // Clean up the video player objects
            Destroy(videoPlayerObject);
            Destroy(canvasObject);
        };

        // Event: When the video fails to load
        videoPlayer.errorReceived += (vp, msg) =>
        {
            Debug.LogError("VideoPlayer Error: " + msg);

            // Keep the black overlay active during the transition to the next scene
            ShowBlackOverlay();

            onVideoComplete?.Invoke();

            // Clean up the video player objects
            Destroy(videoPlayerObject);
            Destroy(canvasObject);
        };

        // Prepare the video
        videoPlayer.Prepare();

        videoPlayer.prepareCompleted += (vp) =>
        {
            Debug.Log("Intro video prepared, starting playback...");
            vp.Play();

            // Hide the black overlay when the video starts playing
            HideBlackOverlay();
        };
    }

    private void ShowBlackOverlay()
    {
        if (blackOverlay == null)
        {
            // Create a black overlay UI Image
            blackOverlay = new GameObject("BlackOverlay");
            Canvas overlayCanvas = blackOverlay.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            UnityEngine.UI.Image overlayImage = blackOverlay.AddComponent<UnityEngine.UI.Image>();
            overlayImage.color = Color.black;

            RectTransform overlayRect = blackOverlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero; // Bottom-left corner
            overlayRect.anchorMax = Vector2.one;  // Top-right corner
            overlayRect.offsetMin = Vector2.zero; // No offset
            overlayRect.offsetMax = Vector2.zero; // No offset
        }

        blackOverlay.SetActive(true);
    }

    private void HideBlackOverlay()
    {
        if (blackOverlay != null)
        {
            blackOverlay.SetActive(false);
        }
    }
}