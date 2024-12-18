using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro; // For TextMeshPro support

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
    private Collider panelToCollect = null; // Tracks the panel to collect
    private float currentFuel = 0f; // Tracks current fuel level
    public GameObject alignmentTask; // Task object for solar panel alignment

    private bool videoPlaying = false; // To track if the video is playing

    private VideoPlayer videoPlayer; // VideoPlayer for level completion
    private GameObject videoCanvas; // Canvas for displaying video

    void Start()
    {
        // Initialize UI and fuel bar
        fuelMeter.fillAmount = 0f;
        alignmentTask.SetActive(false);

        // Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Check for player input to collect a panel
        if (panelToCollect != null && Input.GetKeyDown(KeyCode.E))
        {
            CollectSolarPanel(panelToCollect);
        }

        // If video is playing and player presses Enter, load the next scene
        if (videoPlaying && Input.GetKeyDown(KeyCode.Return))
        {
            LoadNextScene();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SolarPanel"))
        {
            panelToCollect = other;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SolarPanel"))
        {
            panelToCollect = null;
        }
    }

    private void CollectSolarPanel(Collider panel)
    {
        collectedPanels++; // Increment the collected panels count

        // Play collection sound
        if (collectionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectionSound);
        }


        // Update fuel bar
        currentFuel += fuelIncreasePerPanel;
        UpdateFuelMeter(currentFuel);

        // Destroy collected panel
        Destroy(panel.gameObject);

        // If 2 panels collected, play level completion video
        if (collectedPanels == 2 && !videoPlaying)
        {
            Playlevel1Video();
        }

        // If all panels are collected
        if (collectedPanels == totalSolarPanels)
        {
            TriggerAlignmentTask();
        }
    }

    private void UpdateFuelMeter(float fuelValue)
    {
        fuelMeter.fillAmount = Mathf.Clamp01(fuelValue);
    }

        void Playlevel1Video()
    {
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
        // Add a VideoPlayer component to the GameObject
        VideoPlayer videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();
    
        // Set the video path (ensure the video is in the StreamingAssets folder)
        string videoPath = Application.streamingAssetsPath + "/Level 1.mp4";
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
            // Create a Canvas if one doesn't exist
            GameObject canvasObject = new GameObject("GameOverCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.AddComponent<GraphicRaycaster>();
        }
    
        rawImageObject.transform.SetParent(canvas.transform, false);
        rawImage.rectTransform.anchorMin = Vector2.zero; // Bottom-left corner
        rawImage.rectTransform.anchorMax = Vector2.one;  // Top-right corner
        rawImage.rectTransform.offsetMin = Vector2.zero; // No offset
        rawImage.rectTransform.offsetMax = Vector2.zero; // No offset
    
        // Configure VideoPlayer
        videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
        videoPlayer.isLooping = false;
    
        // Attach event listeners for debugging
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
    
        // Prepare the video
        videoPlayer.Prepare();
    }
    void StopAllBackgroundActivities()
    {
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
            rock.enabled = false; // Disable collision handling on rocks
        }
    
        // Stop all rigidbody-based physics (e.g., rocks falling)
        var allRigidbodies = Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
        foreach (var rb in allRigidbodies)
        {
            rb.isKinematic = true; // Disable physics simulation
        }
    
        // Disable all animations
        var allAnimators = Object.FindObjectsByType<Animator>(FindObjectsSortMode.None);
        foreach (var animator in allAnimators)
        {
            animator.enabled = false; // Pause animations
        }
    
        // Disable player movement (if applicable)
        var playerMovement = Object.FindFirstObjectByType<astronaut_controller>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    
        // Optionally freeze any other scripts or game mechanics
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

