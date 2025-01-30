using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System.Collections;

public class WindTurbineCollector : MonoBehaviour
{
    public int totalWindTurbines = 4; // Total number of wind turbines to collect
    private int collectedTurbines = 0; // Number of collected turbines

    public TMP_Text turbineCollectionMessage; // UI Text for turbine collection progress
    public Image fuelMeter; // UI Image for the fuel bar
    public float fuelIncreasePerTurbine = 0.25f; // Fuel bar increment per turbine (4 turbines -> 0.25 per turbine)

    public AudioClip collectionSound; // Sound to play on turbine collection
    public string endVideoFileName = "EndVideo.mp4"; // Name of the video file in StreamingAssets

    private AudioSource audioSource;
    private float currentFuel = 0f; // Tracks current fuel level for wind turbines

    void Start()
    {
        currentFuel = PlayerPrefs.GetFloat("CurrentFuel", 0f);
        collectedTurbines = PlayerPrefs.GetInt("CollectedTurbines", 0); // Fix: Reset value to 0 initially

        // Initialize UI and fuel bar
        fuelMeter.fillAmount = currentFuel;
        UpdateTurbineCollectionMessage();

        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider is tagged as a WindTurbine
        if (other.CompareTag("WindTurbine"))
        {
            CollectWindTurbine(other);
        }
    }

    private void CollectWindTurbine(Collider turbine)
    {
        collectedTurbines++; // Increment collected turbines count

        PlayerPrefs.SetInt("CollectedTurbines", collectedTurbines);

        // Update the UI to reflect the current count
        UpdateTurbineCollectionMessage();

        // Play collection sound
        if (collectionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectionSound);
        }

        // Update the fuel bar
        currentFuel += fuelIncreasePerTurbine;
        UpdateFuelMeter(currentFuel);

        PlayerPrefs.SetFloat("CurrentFuel", currentFuel);

        // Destroy the collected turbine
        Destroy(turbine.gameObject);

        // Check if all turbines are collected and trigger the End Video
        if (collectedTurbines >= totalWindTurbines)
        {
            StartCoroutine(PlayEndVideoWithDelay(1f)); // 1-second delay before playing video
        }
    }

    private void UpdateTurbineCollectionMessage()
    {
        turbineCollectionMessage.text = $"{collectedTurbines} of {totalWindTurbines} Sustainable Assets";
    }

    private void UpdateFuelMeter(float fuelValue)
    {
        fuelMeter.fillAmount = Mathf.Clamp01(fuelValue);
    }

    private IEnumerator PlayEndVideoWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayEndVideo();
    }

    private void PlayEndVideo()
    {
        Debug.Log("Playing End Video...");

        // Create a GameObject for Video Player
        GameObject videoPlayerObject = new GameObject("EndVideoPlayer");
        VideoPlayer videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();

        // Set the path to the video inside StreamingAssets
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, endVideoFileName);
        videoPlayer.url = videoPath;

        // Create a render texture
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        renderTexture.Create();
        videoPlayer.targetTexture = renderTexture;

        // Create a UI Raw Image to display the video
        GameObject rawImageObject = new GameObject("EndVideoRawImage");
        RawImage rawImage = rawImageObject.AddComponent<RawImage>();
        rawImage.texture = renderTexture;

        // Assign it to the Canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("EndVideoCanvas");
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

        // Set Video Properties
        videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
        videoPlayer.isLooping = false;

        // Play Video
        videoPlayer.prepareCompleted += (vp) => { vp.Play(); };
        videoPlayer.loopPointReached += (vp) => { Debug.Log("End Video finished."); };

        videoPlayer.Prepare();
    }
}
