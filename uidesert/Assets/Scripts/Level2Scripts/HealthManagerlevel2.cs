using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video; // For Game Over video functionality

public class HealthManagerLevel2 : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Image healthBarFill;
    public int damageFromStream = 15; // Damage taken per collision with "Stream"
    public AudioClip playerHitSound; // Sound to play on damage
    public AudioClip gameOverSound; // Sound to play on Game Over

    private AudioSource audioSource;
    public GameObject shieldCanvas;
    private bool isGameOver = false; // Ensure Game Over is triggered only once

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Initialize the AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
        audioSource.volume = 1f;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isGameOver) return; // Don't process if Game Over already occurred

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        // Play damage sound effect
        if (playerHitSound != null)
        {
            audioSource.PlayOneShot(playerHitSound);
            Debug.Log("Player hit sound played!");
        }
        else
        {
            Debug.LogWarning("No 'playerHitSound' AudioClip assigned.");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void Die()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Player has died!");

        // Stop all activities
        StopAllBackgroundActivities();

        // Play Game Over sound
        if (gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        // Play the Game Over video
        PlayGameOverVideo();
    }

    void PlayGameOverVideo()

    {

        if (shieldCanvas != null)
        {
            shieldCanvas.SetActive(false);
            Debug.Log("ShieldCanvas disabled.");
        }   
        Debug.Log("Playing Game Over video...");

        GameObject videoPlayerObject = new GameObject("GameOverVideoPlayer");
        VideoPlayer videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();

        string videoPath = Application.streamingAssetsPath + "/Game Over.mp4";
        Debug.Log("Video path: " + videoPath);
        videoPlayer.url = videoPath;

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        renderTexture.Create();
        videoPlayer.targetTexture = renderTexture;

        GameObject rawImageObject = new GameObject("GameOverRawImage");
        UnityEngine.UI.RawImage rawImage = rawImageObject.AddComponent<UnityEngine.UI.RawImage>();
        rawImage.texture = renderTexture;

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

        videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
        videoPlayer.isLooping = false;

        videoPlayer.prepareCompleted += (vp) => { vp.Play(); };
        videoPlayer.loopPointReached += (vp) => { Debug.Log("Game Over video finished."); };

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

        // Disable all Rigidbody physics
        var allRigidbodies = Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
        foreach (var rb in allRigidbodies)
        {
            rb.isKinematic = true;
        }

        // Disable player movement
        var playerMovement = Object.FindFirstObjectByType<astronaut_controller>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        Debug.Log("All background activities stopped.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stream")) // Check if the object has the "Stream" tag
        {
            TakeDamage(damageFromStream);
            Debug.Log("Player entered the stream trigger! Health decreased.");
        }
    }
}

