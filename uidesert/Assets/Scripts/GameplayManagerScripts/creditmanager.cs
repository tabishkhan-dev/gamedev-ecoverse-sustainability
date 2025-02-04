using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CreditVideoPlayer : MonoBehaviour
{
    public Button creditButton; // Assign in Inspector

    private GameObject videoCanvas;
    private VideoPlayer videoPlayer;
    private GameObject backButtonObject; // Store the back button reference

    void Start()
    {
        // Assign button click event
        if (creditButton != null)
        {
            creditButton.onClick.AddListener(PlayCreditVideo);
            
        }
        creditButton.onClick.AddListener(() =>
        {
            Debug.Log("credit Button Clicked!");
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClickSound();
            }
            
        });
    }

    void PlayCreditVideo()
    {
        if (videoCanvas != null)
        {
            Destroy(videoCanvas);
        }

        // Create Video Canvas
        videoCanvas = new GameObject("CreditVideoCanvas");
        Canvas canvas = videoCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        videoCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        videoCanvas.AddComponent<GraphicRaycaster>();

        // Create Video Player
        GameObject videoObject = new GameObject("CreditVideoPlayer");
        videoObject.transform.SetParent(videoCanvas.transform, false);
        videoPlayer = videoObject.AddComponent<VideoPlayer>();

        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, "credit.mp4");
        videoPlayer.url = videoPath;

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        videoPlayer.targetTexture = renderTexture;

        // Create UI RawImage for video display
        GameObject rawImageObject = new GameObject("CreditVideoRawImage");
        rawImageObject.transform.SetParent(videoCanvas.transform, false);
        RawImage rawImage = rawImageObject.AddComponent<RawImage>();
        rawImage.texture = renderTexture;

        RectTransform rt = rawImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // 🔹 Create the Back Button but keep it hidden initially
        backButtonObject = CreateBackButton(videoCanvas, CloseVideo);
        backButtonObject.SetActive(false); // Hide button initially

        // 🔹 Show the Back Button ONLY when the video starts playing
        videoPlayer.started += (vp) => {
            backButtonObject.SetActive(true);
        };

        // Play Video
        videoPlayer.Play();
    }

    private GameObject CreateBackButton(GameObject videoCanvas, System.Action onBack)
    {
        GameObject buttonObject = new GameObject("BackButton");
        buttonObject.transform.SetParent(videoCanvas.transform, false);

        Button button = buttonObject.AddComponent<Button>();
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();

        // Positioning at bottom center
        rectTransform.anchorMin = new Vector2(0.5f, 0.05f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.05f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(70, 20); // Button Size

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = Color.white; // Semi-transparent black

        // Add Text
        GameObject textObject = new GameObject("BackButtonText");
        textObject.transform.SetParent(buttonObject.transform, false);
        Text buttonText = textObject.AddComponent<Text>();
        buttonText.text = "Back";
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.color = Color.black;

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Add click event
        button.onClick.AddListener(() =>
        {
            Debug.Log("Back Button Clicked!");
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClickSound();
            }
            onBack?.Invoke();
        });

        return buttonObject;
    }

    void CloseVideo()
    {
        // Stop and destroy video canvas
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        if (videoCanvas != null)
        {
            Destroy(videoCanvas);
        }

        // Ensure back button is hidden when video is closed
        if (backButtonObject != null)
        {
            backButtonObject.SetActive(false);
        }
    }
}
