using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;

public class QuizManager : MonoBehaviour
{
    public QuizData quizData;

    // UI Elements
    public TMP_Text questionText;
    public TMP_InputField answerInput;
    public TMP_Text feedbackText;
    public TMP_Text timerText;
    public TMP_Text shieldCounterText;
    public Button submitButton;
    public Button proceedButton;
    public Image correctSymbol;
    public Image wrongSymbol;
    public TMP_Text submitButtonText;
    public TMP_Text proceedButtonText;

    public Button hintButton;  // The button that triggers hint display
    public TMP_Text hintText;  // Text element to display the hint

    // Audio Elements
    public AudioClip quizBackgroundMusic; // Background music for the quiz
    public AudioClip correctAnswerSound;  // Sound effect for correct answers
    public AudioClip wrongAnswerSound;    // Sound effect for wrong answers
    public AudioClip levelCompletionSoundl2;
    private bool videoPlaying = false;

    // Video-related variables

    private List<QuizData.Question> randomizedQuestions;
    private int currentQuestionIndex = 0;
    private float timePerQuestion = 30f;
    private float timeRemaining;
    private bool isTimerRunning = false;
    private int shields = 0;
    private AudioSource audioSource;

    public string nextGameSceneName = "GameLevel"; // Name of the game scene to return to

    void Start()
    {
        InitializeQuiz();
        submitButton.onClick.AddListener(CheckAnswer);
        proceedButton.onClick.AddListener(PlayVideoBeforeScene);
    
        // Set initial button text
        if (submitButtonText != null)
            submitButtonText.text = "Submit";
    
        if (proceedButtonText != null)
            proceedButtonText.text = "Proceed";
    
        proceedButton.gameObject.SetActive(false);  // Hide proceed button initially
    
        // Setup audio
        audioSource = gameObject.AddComponent<AudioSource>();
        PlayBackgroundMusic();
    
        // Setup hint button
        if (hintButton != null)
            hintButton.onClick.AddListener(ShowHint);
    }
    
    void Update()
    {
        // If video is playing and player presses Enter, load the next scene
        if (videoPlaying && Input.GetKeyDown(KeyCode.Return))
        {
            LoadNextGameScene();
        }
    }
    
    void QuizComplete()
    {
        questionText.text = "Quiz Complete!";
        feedbackText.text = "";
        timerText.text = "";  // Clear the timer
        shieldCounterText.text = "";  // Clear the shield counter
    
        submitButton.gameObject.SetActive(false);
        answerInput.gameObject.SetActive(false);  // Hide the answer input field
        correctSymbol.gameObject.SetActive(false);
        wrongSymbol.gameObject.SetActive(false);
    
        proceedButton.gameObject.SetActive(true);  // Show the proceed button
    }
    
    void PlayVideoBeforeScene()
    {
        proceedButton.gameObject.SetActive(false);  // Hide the proceed button
        Debug.Log("Initializing level2 video...");
        videoPlaying = true;
    
        // Play level completion sound
        if (levelCompletionSoundl2 != null)
        {
            audioSource.Stop(); // Stop any other audio
            audioSource.clip = levelCompletionSoundl2;
            audioSource.Play();
        }
    
        // Create a new GameObject for the VideoPlayer
        GameObject videoPlayerObject = new GameObject("quizcompletedvideo");
        VideoPlayer videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();
    
        // Set the video path (ensure the video is in the StreamingAssets folder)
        string videoPath = Application.streamingAssetsPath + "/lvl_2.mp4";
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
            Debug.Log("Level2 video prepared, starting playback...");
            vp.Play();
        };
    
        videoPlayer.errorReceived += (vp, msg) =>
        {
            Debug.LogError("VideoPlayer Error: " + msg);
        };
    
        videoPlayer.loopPointReached += (vp) =>
        {
            Debug.Log("Level2 video finished playing.");
            videoPlaying = false;  // Video finished
        };
    
        // Prepare the video
        videoPlayer.Prepare();
    }


    void InitializeQuiz()
    {
        string keysString = PlayerPrefs.GetString("SelectedFactKeys", "");
        Debug.Log("Retrieved Keys from PlayerPrefs: " + keysString);

        List<string> selectedKeys = new List<string>(keysString.Split(','));

        // Filter questions based on selected keys
        randomizedQuestions = new List<QuizData.Question>();
        foreach (var question in quizData.questions)
        {
            if (selectedKeys.Contains(question.knowledgeFact)) // Assuming "knowledgeFact" holds the key
            {
                randomizedQuestions.Add(question);
            }
        }

        // Randomize and select 3 questions
        RandomizeQuestions();
        shields = 0;
        UpdateShieldCounter();
        DisplayQuestion();
    }

    void RandomizeQuestions()
    {
        for (int i = 0; i < randomizedQuestions.Count; i++)
        {
            int randomIndex = Random.Range(i, randomizedQuestions.Count);
            var temp = randomizedQuestions[i];
            randomizedQuestions[i] = randomizedQuestions[randomIndex];
            randomizedQuestions[randomIndex] = temp;
        }

        randomizedQuestions = randomizedQuestions.GetRange(1, Mathf.Min(3, randomizedQuestions.Count));
    }

    void DisplayQuestion()
    {
        // Hide the symbols when moving to the next question
        correctSymbol.gameObject.SetActive(false);
        wrongSymbol.gameObject.SetActive(false);

        if (currentQuestionIndex < randomizedQuestions.Count)
        {
            questionText.text = randomizedQuestions[currentQuestionIndex].questionText;
            feedbackText.text = "";
            answerInput.text = "";

            // Hide hint text at the start
            hintText.text = "";

            // Restart the timer
            timeRemaining = timePerQuestion;
            isTimerRunning = true;
            StartCoroutine(UpdateTimer());
        }
        else
        {
            Debug.Log("All questions answered. Completing the quiz.");
            QuizComplete();
        }
    }

    IEnumerator UpdateTimer()
    {
        while (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timerText.text = $"Time Left: {Mathf.CeilToInt(timeRemaining)}s";
            }
            else
            {
                isTimerRunning = false;
                feedbackText.text = "Time's up!";
                ShowSymbol(wrongSymbol);
                PlayWrongSound();
                currentQuestionIndex++;
                Invoke(nameof(DisplayQuestion), 2);
            }
            yield return null;
        }
    }

    void CheckAnswer()
    {
        if (!isTimerRunning) return;

        string userAnswer = answerInput.text.Trim().ToLower(); // Normalize input: trim spaces & lowercase

        // Fetch the list of valid answers for the current question
        List<string> correctAnswers = randomizedQuestions[currentQuestionIndex].correctAnswers;

        bool isCorrect = false;

        // Check if the user's answer matches any correct answer (case-insensitive)
        foreach (string correctAnswer in correctAnswers)
        {
            if (userAnswer.Equals(correctAnswer.Trim().ToLower()))
            {
                isCorrect = true;
                break;
            }
        }

        // Provide feedback based on correctness
        if (isCorrect)
        {
            feedbackText.text = "Correct!";
            ShowSymbol(correctSymbol);
            PlayCorrectSound();
            shields++;
        }
        else
        {
            feedbackText.text = "Wrong Answer!";
            ShowSymbol(wrongSymbol);
            PlayWrongSound();
        }

        UpdateShieldCounter();
        isTimerRunning = false;
        currentQuestionIndex++;
        Invoke(nameof(DisplayQuestion), 2); // Delay before moving to the next question
    }

    void ShowSymbol(Image symbol)
    {
        correctSymbol.gameObject.SetActive(false);
        wrongSymbol.gameObject.SetActive(false);

        symbol.gameObject.SetActive(true);
        Invoke(nameof(HideSymbols), 1f);  // Delay to hide symbols after 1 second
    }

    void HideSymbols()
    {
        correctSymbol.gameObject.SetActive(false);
        wrongSymbol.gameObject.SetActive(false);
    }

    void UpdateShieldCounter()
    {
        shieldCounterText.text = $"Shields:{shields}/3 🛡"; // Update the shield counter
    }

    

    void LoadNextGameScene()
    {
        PlayerPrefs.SetInt("ShieldCount", shields); // Save the shield count
        PlayerPrefs.Save(); // Ensure the data is written to disk
        SceneManager.LoadScene(nextGameSceneName);  // Load the next game scene
    }

    // Audio Methods
    void PlayBackgroundMusic()
    {
        if (audioSource != null && quizBackgroundMusic != null)
        {
            audioSource.clip = quizBackgroundMusic;
            audioSource.loop = true;  // Loop background music
            audioSource.Play();
        }
    }

    void PlayCorrectSound()
    {
        if (audioSource != null && correctAnswerSound != null)
        {
            audioSource.PlayOneShot(correctAnswerSound);
        }
    }

    void PlayWrongSound()
    {
        if (audioSource != null && wrongAnswerSound != null)
        {
            audioSource.PlayOneShot(wrongAnswerSound);
        }
    }

    // New method for showing the hint
    void ShowHint()
    {
        if (currentQuestionIndex < randomizedQuestions.Count)
        {
            string hint = randomizedQuestions[currentQuestionIndex].hint;  // Get the hint for the current question
            
            if (!string.IsNullOrEmpty(hint))
            {
                hintText.text = hint;  // Display the hint text
            }
            else
            {
                hintText.text = "No hint available.";  // Default text if no hint is available
            }
        }
    }
}
