using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameMenuManager : MonoBehaviour
{
    public string introVideoName = "IntroStory.mp4"; // Video file in StreamingAssets
    private GameObject blackOverlay;
    private GameObject skipButton; // Skip Button reference
    private VideoPlayer videoPlayer;

    public GameObject gameMenuAudio; // Reference to the "gamemenu" audio object
    private AudioSource audioSource; // To play button click sounds

    public void StartGame()
    {
        ResetProgress();
        ShowBlackOverlay();

        GameObject mainCanvas = GameObject.FindWithTag("mainCanvas");
        if (mainCanvas != null)
        {
            mainCanvas.SetActive(false); // Hide main menu
        }

        PlayIntroVideo(() =>
        {
            SceneManager.LoadScene("SampleScene"); // Load Level 1
        });
    }

    private void ResetProgress()
    {
        PlayerPrefs.DeleteKey("CollectedSolarPanels");
        PlayerPrefs.DeleteKey("CollectedTurbines");
        PlayerPrefs.DeleteKey("CurrentFuel");
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.Save();
    }

    private void PlayIntroVideo(System.Action onVideoComplete)
    {
        Debug.Log("Initializing Intro video...");

        // Disable game menu audio
        if (gameMenuAudio != null)
        {
            gameMenuAudio.SetActive(false);
        }

        // ✅ Create Video Player
        GameObject videoPlayerObject = new GameObject("IntroVideoPlayer");
        videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();

        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, introVideoName);
        videoPlayer.url = videoPath;

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        renderTexture.Create();
        videoPlayer.targetTexture = renderTexture;

        // ✅ Create Video Canvas
        GameObject videoCanvasObject = new GameObject("IntroVideoCanvas");
        Canvas videoCanvas = videoCanvasObject.AddComponent<Canvas>();
        videoCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        videoCanvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        videoCanvasObject.AddComponent<GraphicRaycaster>();

        // ✅ Create Raw Image to Display Video
        GameObject rawImageObject = new GameObject("IntroVideoRawImage");
        RawImage rawImage = rawImageObject.AddComponent<RawImage>();
        rawImage.texture = renderTexture;
        rawImage.transform.SetParent(videoCanvasObject.transform, false);
        rawImage.rectTransform.anchorMin = Vector2.zero;
        rawImage.rectTransform.anchorMax = Vector2.one;
        rawImage.rectTransform.offsetMin = Vector2.zero;
        rawImage.rectTransform.offsetMax = Vector2.zero;

        // ✅ Create Skip Button inside Video Canvas
        skipButton = CreateSkipButton(videoCanvasObject);
        skipButton.SetActive(false); // Hide initially

        videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
        videoPlayer.isLooping = false;

        videoPlayer.loopPointReached += (vp) =>
        {
            Debug.Log("Intro video finished playing.");
            onVideoComplete?.Invoke();
            Destroy(videoPlayerObject);
            Destroy(videoCanvasObject);

            // Re-enable game menu audio
            if (gameMenuAudio != null)
            {
                gameMenuAudio.SetActive(true);
            }
        };

        videoPlayer.errorReceived += (vp, msg) =>
        {
            Debug.LogError("VideoPlayer Error: " + msg);
            onVideoComplete?.Invoke();
            Destroy(videoPlayerObject);
            Destroy(videoCanvasObject);

            // Re-enable game menu audio
            if (gameMenuAudio != null)
            {
                gameMenuAudio.SetActive(true);
            }
        };

        videoPlayer.Prepare();

        videoPlayer.prepareCompleted += (vp) =>
        {
            Debug.Log("Intro video prepared, starting playback...");
            vp.Play();
            skipButton.SetActive(true); // ✅ Show Skip Button when video starts
            HideBlackOverlay();
        };
    }

    private GameObject CreateSkipButton(GameObject videoCanvas)
    {
        GameObject buttonObject = new GameObject("SkipButton");
        buttonObject.transform.SetParent(videoCanvas.transform, false);

        Button button = buttonObject.AddComponent<Button>();
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.85f, 0.05f); // Bottom-right corner
        rectTransform.anchorMax = new Vector2(0.95f, 0.12f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        // ✅ Add Button Background for Visibility
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0, 0, 0, 0.6f); // Semi-transparent black background

        // ✅ Add Button Text with Proper Font
        GameObject textObject = new GameObject("ButtonText");
        textObject.transform.SetParent(buttonObject.transform, false);
        Text buttonText = textObject.AddComponent<Text>();
        buttonText.text = "Skip";
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // ✅ Fix font issue
        buttonText.color = Color.white;

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        button.onClick.AddListener(() =>
        {
            Debug.Log("Skip Button Clicked!");
            PlayButtonClickSound();
            videoPlayer.Stop();
            SceneManager.LoadScene("SampleScene"); // Load Level 1

            // Re-enable game menu audio
            if (gameMenuAudio != null)
            {
                gameMenuAudio.SetActive(true);
            }
        });

        return buttonObject;
    }

    private void PlayButtonClickSound()
    {
        if (audioSource == null)
        {
            audioSource = gameMenuAudio.GetComponent<AudioSource>();
        }

        if (audioSource != null)
        {
            audioSource.Play(); // Play the button click sound
        }
        else
        {
            Debug.LogError("AudioSource component not found on gameMenuAudio object!");
        }
    }

    private void ShowBlackOverlay()
    {
        if (blackOverlay == null)
        {
            blackOverlay = new GameObject("BlackOverlay");
            Canvas overlayCanvas = blackOverlay.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            Image overlayImage = blackOverlay.AddComponent<Image>();
            overlayImage.color = Color.black;

            RectTransform overlayRect = blackOverlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
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
