using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameMenuManager : MonoBehaviour
{
    public string introVideoName = "IntroStory.mp4"; 
    public string welcomeVideoName = "welcomeL1.mp4"; 

    private GameObject blackOverlay;
    private GameObject skipButton;
    private VideoPlayer videoPlayer;
    private bool isWelcomeVideoPlaying = false; 
    private GameObject welcomeVideoCanvas = null; //  Store Welcome Video Canvas

    public GameObject gameMenuAudio; 
    public GameObject welcomeL1Audio; 

    private void StartGame()
    {
        ResetProgress();
        ShowBlackOverlay();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }

        GameObject mainCanvas = GameObject.FindWithTag("mainCanvas");
        if (mainCanvas != null)
        {
            mainCanvas.SetActive(false);
        }

        PlayVideo(introVideoName, () =>
        {
            PlayVideo(welcomeVideoName, () =>
            {
                Debug.Log("WelcomeL1.mp4 ended, waiting for Enter key...");
                isWelcomeVideoPlaying = true; //  Keep Welcome video on screen
            }, welcomeL1Audio, true);
        }, null);
    }

    private void ResetProgress()
    {
        PlayerPrefs.DeleteKey("CollectedSolarPanels");
        PlayerPrefs.DeleteKey("CollectedTurbines");
        PlayerPrefs.DeleteKey("CurrentFuel");
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.Save();
    }

    private void PlayVideo(string videoFileName, System.Action onVideoComplete, GameObject videoAudio, bool enableEnterKey = false)
    {
        Debug.Log("Initializing video: " + videoFileName);

        //  Disable game menu audio instead of just muting
        if (gameMenuAudio != null) gameMenuAudio.SetActive(false);
        if (welcomeL1Audio != null) welcomeL1Audio.SetActive(false);

        GameObject videoPlayerObject = new GameObject(videoFileName + "Player");
        videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();

        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.url = videoPath;

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        renderTexture.Create();
        videoPlayer.targetTexture = renderTexture;

        GameObject videoCanvasObject = new GameObject(videoFileName + "Canvas");
        Canvas videoCanvas = videoCanvasObject.AddComponent<Canvas>();
        videoCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        videoCanvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        videoCanvasObject.AddComponent<GraphicRaycaster>();

        GameObject rawImageObject = new GameObject(videoFileName + "RawImage");
        RawImage rawImage = rawImageObject.AddComponent<RawImage>();
        rawImage.texture = renderTexture;
        rawImage.transform.SetParent(videoCanvasObject.transform, false);
        rawImage.rectTransform.anchorMin = Vector2.zero;
        rawImage.rectTransform.anchorMax = Vector2.one;
        rawImage.rectTransform.offsetMin = Vector2.zero;
        rawImage.rectTransform.offsetMax = Vector2.zero;

        if (videoFileName == introVideoName)
        {
            skipButton = CreateSkipButton(videoCanvasObject, () =>
            {
                videoPlayer.Stop();
                PlayVideo(welcomeVideoName, () =>
                {
                    Debug.Log("WelcomeL1.mp4 ended, waiting for Enter key..."); 
                    isWelcomeVideoPlaying = true;
                }, welcomeL1Audio, true);
            });
            skipButton.SetActive(false);
        }

        videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
        videoPlayer.isLooping = false;

        videoPlayer.loopPointReached += (vp) =>
        {
            Debug.Log(videoFileName + " finished playing.");

            if (videoFileName == welcomeVideoName)
            {
                //  Keep Welcome Video canvas visible
                isWelcomeVideoPlaying = true;
                welcomeVideoCanvas = videoCanvasObject;
                return;
            }

            ShowBlackOverlay();
            onVideoComplete?.Invoke();
            Destroy(videoPlayerObject);
            Destroy(videoCanvasObject);

            if (gameMenuAudio != null && !isWelcomeVideoPlaying && videoFileName != introVideoName) 
                gameMenuAudio.SetActive(true);

        };

        videoPlayer.errorReceived += (vp, msg) =>
        {
            Debug.LogError("VideoPlayer Error: " + msg);
            onVideoComplete?.Invoke();
            Destroy(videoPlayerObject);
            Destroy(videoCanvasObject);

            if (gameMenuAudio != null) gameMenuAudio.SetActive(true);
            isWelcomeVideoPlaying = false;
        };

        videoPlayer.Prepare();

        videoPlayer.prepareCompleted += (vp) =>
        {
            Debug.Log(videoFileName + " prepared, starting playback...");

            bool isMuted = PlayerPrefs.GetInt("StoryVolume", 0) == 1;
            videoPlayer.SetDirectAudioMute(0, isMuted);

            vp.Play();
            if (videoFileName == introVideoName) skipButton.SetActive(true);
            HideBlackOverlay();

            if (videoAudio != null) videoAudio.SetActive(true);

            isWelcomeVideoPlaying = enableEnterKey;
        };
    }

    private GameObject CreateSkipButton(GameObject videoCanvas, System.Action onSkip)
    {
        GameObject buttonObject = new GameObject("SkipButton");
        buttonObject.transform.SetParent(videoCanvas.transform, false);

        Button button = buttonObject.AddComponent<Button>();
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.85f, 0.05f);
        rectTransform.anchorMax = new Vector2(0.95f, 0.12f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0, 0, 0, 0.6f);

        GameObject textObject = new GameObject("ButtonText");
        textObject.transform.SetParent(buttonObject.transform, false);
        Text buttonText = textObject.AddComponent<Text>();
        buttonText.text = "Skip";
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.color = Color.white;

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        button.onClick.AddListener(() =>
        {
            Debug.Log("Skip Button Clicked!");
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClickSound();
            }
            onSkip?.Invoke();
        });

        return buttonObject;
    }

    private void Update()
    {
        if (isWelcomeVideoPlaying && Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Enter key pressed! Loading game...");
            isWelcomeVideoPlaying = false;

            if (welcomeVideoCanvas != null) Destroy(welcomeVideoCanvas); // Remove welcome video when Enter is pressed
           
            
            LoadGameScene();
        }
    }

    private void LoadGameScene()
    {
        ShowBlackOverlay();
        Debug.Log("Loading SampleScene...");
        SceneManager.LoadScene("SampleScene");
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
