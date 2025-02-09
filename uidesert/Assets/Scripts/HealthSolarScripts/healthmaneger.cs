    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Video; // Import Video namespace

    public class HealthManager : MonoBehaviour
    {
        public int maxHealth = 100;
        public int currentHealth;
        public Image healthBarFill;
        public int damageFromRock = 20; // Amount of damage taken per collision
        public AudioClip playerHitSound; // "Aah" sound clip
        public AudioClip gameOverSound; // Sound to play on Game Over

        private AudioSource audioSource; // Audio source for playing sounds
        private bool isGameOver = false; // To ensure Game Over logic is executed once

        void Start()
        {
            currentHealth = maxHealth;
            UpdateHealthBar();

            // Add an AudioSource component to the player if not already present
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false; // Prevent sound from playing on spawn
            audioSource.spatialBlend = 0f;   // Set to 2D sound
            audioSource.volume = 1f;        // Ensure volume is not muted
        }

        public void TakeDamage(int damageAmount)
        {
            if (isGameOver) return; // Stop processing if the game is already over

            currentHealth -= damageAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthBar();

            PlayerPrefs.SetInt("PlayerHealth", currentHealth);
            PlayerPrefs.Save();

            // Play the "aah" sound when damage is taken
            if (playerHitSound != null)
            {
                audioSource.PlayOneShot(playerHitSound);
                //Debug.Log("Aah sound played for damage!");
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

        public void AddHealth(int healthAmount)
        {
            if (isGameOver) return; // Prevent health addition after game over

            currentHealth += healthAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't exceed max

            UpdateHealthBar(); // Refresh health UI
            PlayerPrefs.SetInt("PlayerHealth", currentHealth);
            PlayerPrefs.Save();

            //Debug.Log("Health increased! Current health: " + currentHealth);
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
            if (isGameOver) return; // Ensure this logic runs only once
            isGameOver = true;

            Debug.Log("Player Died!");

            // Stop all background activities
            StopAllBackgroundActivities();

            // Play the Game Over sound
            if (gameOverSound != null)
            {
                audioSource.PlayOneShot(gameOverSound);
            }

            // Display the Game Over video
            PlayGameOverVideo();

            // Additional logic like restarting or returning to the menu can be added here
        }

        void PlayGameOverVideo()
        {
            Debug.Log("Initializing Game Over video...");

            
        
            // Create a new GameObject for the VideoPlayer
            GameObject videoPlayerObject = new GameObject("GameOverVideoPlayer");
        
            // Add a VideoPlayer component to the GameObject
            VideoPlayer videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();
        
            // Set the video path (ensure the video is in the StreamingAssets folder)
            string videoPath = Application.streamingAssetsPath + "/gameoverFinal.mp4";
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
            // Adjust RectTransform to cover the entire screen
            RectTransform rectTransform = rawImage.rectTransform;
            rectTransform.anchorMin = Vector2.zero; // Bottom-left corner
            rectTransform.anchorMax = Vector2.one;  // Top-right corner
            rectTransform.offsetMin = Vector2.zero; // No offset
            rectTransform.offsetMax = Vector2.zero; // No offset
            rectTransform.sizeDelta = Vector2.zero; // Ensure no extra spacing
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

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Obstacle")) // Check if the collided object is tagged as 'Rock'
            {
                TakeDamage(damageFromRock);
                Debug.Log("Player hit by a rock! Health decreased.");
            }
        }
        
    }
