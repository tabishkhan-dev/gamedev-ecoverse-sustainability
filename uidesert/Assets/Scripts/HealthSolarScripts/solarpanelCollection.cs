using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro; // For TextMeshPro support
using System.Collections; // Required for IEnumerator

public class SolarPanelCollector : MonoBehaviour
{
    public int totalSolarPanels = 8; // Total number of solar panels to collect
    private int collectedPanels = 0; // Number of panels collected

    public TMP_Text collectionMessage; // UI Text to display panel collection progress
    public Image fuelMeter; // UI Image for the fuel bar
    public float fuelIncreasePerPanel = 0.125f; // Fuel bar increment per panel

    public AudioClip collectionSound; // Sound to play when a panel is collected
    public AudioClip levelCompletionSound; // Sound for level completion

    private AudioSource audioSource;
    //private Collider panelToCollect = null; // Tracks the panel to collect
    private float currentFuel = 0f; // Tracks current fuel level
    public GameObject alignmentTask; // Task object for solar panel alignment

    private bool videoPlaying = false; // To track if the video is playing

    private VideoPlayer videoPlayer; // VideoPlayer for level completion
    //private GameObject videoCanvas; // Canvas for displaying video

    public AudioClip clickSound;
    
    public StoryVolumeManager storyVolumeManager;

    private string savedLanguage;

    public string levelcompletedvideo = "lvl_1CompleteHindi.mp4";


    void Start()
    {

         // Retrieve the saved language from PlayerPrefs
        savedLanguage = PlayerPrefs.GetString("Language", "English");
        Debug.Log("Loaded language in Start(): " + savedLanguage);
        // Initialize UI and fuel bar
        collectedPanels = PlayerPrefs.GetInt("CollectedSolarPanels", 0);
        currentFuel = PlayerPrefs.GetFloat("CurrentFuel", 0f);
        fuelMeter.fillAmount = currentFuel;

        alignmentTask.SetActive(false);

        // Update the collection message UI
        UpdateCollectionMessage();

        PlayerPrefs.SetInt("videoPlaying", 0);
        PlayerPrefs.Save();

        // Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        //Debug.Log($"Video Playing State: {videoPlaying}");

        // Allow Enter key press ONLY when the video is playing
        if (videoPlaying && Input.GetKeyDown(KeyCode.Return))
        {
            PlayClickSound(); // Play the click sound
            Invoke(nameof(LoadNextScene), 0.1f); // Delay scene switch to allow sound to play
        }
    }

    void PlayClickSound()
    {
        if (audioSource != null && collectionSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SolarPanel"))
        {
            CollectSolarPanel(other);
        }
    }

    private void CollectSolarPanel(Collider panel)
    {
        collectedPanels++; // Increment the collected panels count

        PlayerPrefs.SetInt("CollectedSolarPanels", collectedPanels);

        // Update the UI to reflect the current count
        UpdateCollectionMessage();

        // Play collection sound
        if (collectionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectionSound);
        }

        // Update fuel bar
        currentFuel += fuelIncreasePerPanel;
        UpdateFuelMeter(currentFuel);

        PlayerPrefs.SetFloat("CurrentFuel", currentFuel);

        // Destroy collected panel
        Destroy(panel.gameObject);

        // If 4 panels collected, play level completion video with a 1-second delay
        if (collectedPanels == 4 && !videoPlaying)
        {
            StartCoroutine(PlayLevel1VideoWithDelay());
        }

        // If all panels are collected
        if (collectedPanels == totalSolarPanels)
        {
            TriggerAlignmentTask();
        }
    }

    private void UpdateCollectionMessage()
    {
        collectionMessage.text = $"{collectedPanels} of 8 Sustainable Assets";
    }

    private void UpdateFuelMeter(float fuelValue)
    {
        fuelMeter.fillAmount = Mathf.Clamp01(fuelValue);
    }

    private IEnumerator PlayLevel1VideoWithDelay()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second
        Playlevel1Video();
    }

    void Playlevel1Video()
    {
        PlayerPrefs.SetString("Language", savedLanguage);
        PlayerPrefs.Save(); // Ensure changes are written
    
        Debug.Log("Selected language saved as: " + savedLanguage);
        
        Debug.Log("Initializing level1 video...");
        videoPlaying = true;

        // Stop all background sounds and animations
        StopAllBackgroundActivities();

        // Play level completion sound
        if (levelCompletionSound != null)
        {
            audioSource.Stop(); // Stop any other audio
            audioSource.clip = levelCompletionSound;
            audioSource.Play();
        }

        // Create a new GameObject for the VideoPlayer
        GameObject videoPlayerObject = new GameObject("levelcompletedvideo");
        VideoPlayer videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();

        string videoFileName = savedLanguage == "Hindi" ? "lvl_1CompleteHindi.mp4" : "lvl_1Complete.mp4";

    // Set the video path (ensure the video is in the StreamingAssets folder)
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        

        // Set the video path (ensure the video is in the StreamingAssets folder)
        
        Debug.Log("Video path: " + videoPath);
        videoPlayer.url = videoPath;

        // Create a Render Texture for the video
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        renderTexture.Create();

        // Assign the Render Texture to the VideoPlayer
        videoPlayer.targetTexture = renderTexture;

        // Create a new UI Image to display the Render Texture
        GameObject rawImageObject = new GameObject("GameOverRawImage");
        UnityEngine.UI.RawImage rawImage = rawImageObject.AddComponent<UnityEngine.UI.RawImage>();

        // Set the Render Texture as the source for the RawImage
        rawImage.texture = renderTexture;

        // Attach the RawImage to a Canvas for full-screen display
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("GameOverCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        rawImageObject.transform.SetParent(canvas.transform, false);
        rawImage.rectTransform.anchorMin = Vector2.zero;
        rawImage.rectTransform.anchorMax = Vector2.one;
        rawImage.rectTransform.offsetMin = Vector2.zero;
        rawImage.rectTransform.offsetMax = Vector2.zero;

        // Configure VideoPlayer
        videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
        videoPlayer.isLooping = false;

        

        // Attach event listeners
        videoPlayer.prepareCompleted += (vp) =>
        {
            Debug.Log("Game Over video prepared, starting playback...");
            vp.Play();
        };

        videoPlayer.errorReceived += (vp, msg) =>
        {
            Debug.LogError("VideoPlayer Error: " + msg);
        };

        videoPlayer.loopPointReached += (vp) =>
        {
            Debug.Log("Game Over video finished playing.");
        };

        bool isMuted = storyVolumeManager != null && PlayerPrefs.GetInt(StoryVolumeManager.StoryVolumeKey, 0) == 1;
        videoPlayer.SetDirectAudioMute(0, isMuted);

        // Prepare the video
        videoPlayer.Prepare();
    }

    void StopAllBackgroundActivities()
    {
        PlayerPrefs.SetInt("videoPlaying", 1);
        PlayerPrefs.Save();
        // Stop all sounds
        var allAudioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var source in allAudioSources)
        {
            source.Stop();
        }

        // Disable all rock interactions
        var rocks = Object.FindObjectsByType<RockCollisionHandler>(FindObjectsSortMode.None);
        foreach (var rock in rocks)
        {
            rock.enabled = false;
        }

        // Stop all rigidbody-based physics
        var allRigidbodies = Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
        foreach (var rb in allRigidbodies)
        {
            rb.isKinematic = true;
        }

        // Disable all animations
        var allAnimators = Object.FindObjectsByType<Animator>(FindObjectsSortMode.None);
        foreach (var animator in allAnimators)
        {
            animator.enabled = false;
        }

        // Disable player movement
        var playerMovement = Object.FindFirstObjectByType<astronaut_controller>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        Debug.Log("All background activities have been stopped.");
    }

    private void TriggerAlignmentTask()
    {
        Debug.Log("All solar panels collected! Prepare to align them towards the sun.");
        alignmentTask.SetActive(true);
        collectionMessage.text = "Align the solar panels to the sun!";
    }

    private void LoadNextScene()
    {
        Debug.Log("Loading Next Scene...");
        SceneManager.LoadScene("QuizLevel"); // Replace with the correct scene name
    }
}
