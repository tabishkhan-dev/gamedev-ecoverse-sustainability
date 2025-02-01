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
        public GameObject quizCompleteTextObject;

        // UI Elements
        public TMP_Text questionText;
        public TMP_InputField answerInput;
        public TMP_Text feedbackText;
        public TMP_Text timerText;
        public TMP_Text shieldCounterText;
        public Button submitButton;
        public Button proceedButton;
     //   public Image correctSymbol;
        
    //    public Image wrongSymbol;
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

        public TimerIcon timerIcon;
        public Image  shieldIcon;

                

        void Start()
    {
        InitializeQuiz();
        if (quizCompleteTextObject != null)
        {
            quizCompleteTextObject.SetActive(false); // Ensure it's hidden at the start
        }
        submitButton.onClick.AddListener(CheckAnswer);

        proceedButton.gameObject.SetActive(false);
        proceedButton.onClick.AddListener(() =>
        {
            if (!videoPlaying)
            {
                PlayVideoBeforeScene();
            }
        });

        if (submitButtonText != null)
            submitButtonText.text = "GO";

        if (proceedButtonText != null)
            proceedButtonText.text = "CONTINUE";

        audioSource = gameObject.AddComponent<AudioSource>();
        PlayBackgroundMusic();

        if (hintButton != null)
            hintButton.onClick.AddListener(ShowHint);

        videoPlaying = false;

        // Ensure the timer text starts with black color
        timerText.color = Color.green;

        //timerText.text = "<color=black>Time Left:</color> ";
    }



        
        void Update()
    {
        Debug.Log($"Video Playing State: {videoPlaying}");

        // Allow Enter key press ONLY when the video is playing
        if (videoPlaying && Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Enter key pressed after video finished, loading next game scene...");
            LoadNextGameScene();
        }
    }



        void QuizComplete()
{
    Debug.Log("Quiz Complete!");

    // Hide all question-related UI elements
    questionText.text = "";
    feedbackText.text = "";
    timerText.text = "";
    shieldCounterText.text = "";
    answerInput.gameObject.SetActive(false);
    submitButton.gameObject.SetActive(false);
    hintButton.gameObject.SetActive(false);
    hintText.gameObject.SetActive(false);
    timerIcon.gameObject.SetActive(false);
    shieldIcon.gameObject.SetActive(false);

    // Show the Quiz Complete UI
    if (quizCompleteTextObject != null)
    {
        quizCompleteTextObject.SetActive(true);
        StartCoroutine(AnimateProcessingText()); // Start dots animation
    }

    // Enable proceed button to go to the next level
    proceedButton.gameObject.SetActive(true);
}



        
        void PlayVideoBeforeScene()
    {
        // Ensure quiz is completed before playing the video
        if (!proceedButton.gameObject.activeSelf)
        {
            Debug.Log("Cannot play video before completing the quiz!");
            return;  // Stop execution if quiz is not yet completed
        }

        proceedButton.gameObject.SetActive(false);  // Hide proceed button
        Debug.Log("Initializing level2 video...");
        videoPlaying = true;  // Enable Enter key detection only during video playback

        // Play level completion sound
        if (levelCompletionSoundl2 != null)
        {
            audioSource.Stop(); // Stop other audio
            audioSource.clip = levelCompletionSoundl2;
            audioSource.Play();
        }

        // Create a new GameObject for the VideoPlayer
        GameObject videoPlayerObject = new GameObject("quizcompletedvideo");
        VideoPlayer videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();

        // Set the video path (ensure the video is in the StreamingAssets folder)
        string videoPath = Application.streamingAssetsPath + "/lvl_2Complete.mp4";
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
            videoPlaying = true;  // Enable Enter key press only after video is finished
            Debug.Log("Press Enter to proceed to the next game scene.");
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
    if (currentQuestionIndex >= randomizedQuestions.Count)
    {
        Debug.Log("All questions answered. Completing the quiz.");
        QuizComplete();
        return;
    }

    // Hide symbols
    //correctSymbol.gameObject.SetActive(false);
    //wrongSymbol.gameObject.SetActive(false);

    // Set question and reset answer
    questionText.text = randomizedQuestions[currentQuestionIndex].questionText;
    feedbackText.text = "";
    answerInput.text = "";

    // Reset input field color to white
    Color whiteColor;
    ColorUtility.TryParseHtmlString("#FFFFFF", out whiteColor);
    answerInput.colors = ChangeInputFieldColor(whiteColor);

    // Hide hint text
    hintText.text = "";

    // Restart QuizManager Timer
    timeRemaining = timePerQuestion;
    isTimerRunning = true;
    StartCoroutine(UpdateTimer());

    // Restart TimerIcon Timer
    if (timerIcon != null)
    {
        timerIcon.RestartTimer();
    }
}



    private ColorBlock ChangeInputFieldColor(Color newColor)
{
    ColorBlock colors = answerInput.colors;
    colors.normalColor = newColor;
    colors.highlightedColor = newColor;
    colors.selectedColor = newColor;
    colors.pressedColor = newColor;
    return colors;
}


        

        IEnumerator UpdateTimer()
{
    while (isTimerRunning)
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = $"{Mathf.CeilToInt(timeRemaining)}s";

            // Set timer text color based on time remaining
            timerText.color = timeRemaining > 9 ? Color.green : Color.red;
        }
        else
        {
            isTimerRunning = false;
            feedbackText.text = "Time's up!";
            //ShowSymbol(wrongSymbol);
            PlayWrongSound();

            // Change input field color to red (wrong answer)
            Color wrongColor;
            ColorUtility.TryParseHtmlString("#F68C8C", out wrongColor);
            answerInput.colors = ChangeInputFieldColor(wrongColor);

            currentQuestionIndex++;
            Invoke(nameof(DisplayQuestion), 2);
        }
        yield return null;
    }
}



void CheckAnswer()
{
    if (!isTimerRunning) return;

    // Normalize input: trim spaces, lowercase, and remove all spaces between words
    string userAnswer = answerInput.text.Trim().ToLower().Replace(" ", "");

    // Fetch the list of valid answers for the current question
    List<string> correctAnswers = randomizedQuestions[currentQuestionIndex].correctAnswers;

    // Normalize each correct answer: lowercase and remove spaces
    bool isCorrect = correctAnswers.Exists(answer => answer.Trim().ToLower().Replace(" ", "") == userAnswer);

    // Define correct (green) and wrong (red) colors
    Color correctColor, wrongColor;
    ColorUtility.TryParseHtmlString("#96E49B", out correctColor); // Green
    ColorUtility.TryParseHtmlString("#F68C8C", out wrongColor);   // Red

    // Change input field color based on correctness
    answerInput.colors = ChangeInputFieldColor(isCorrect ? correctColor : wrongColor);

    // Provide feedback based on correctness
    if (isCorrect)
    {
        feedbackText.text = "Correct!";
        //ShowSymbol(correctSymbol);
        PlayCorrectSound();
        shields++;
    }
    else
    {
        feedbackText.text = "Wrong Answer!";
        //ShowSymbol(wrongSymbol);
        PlayWrongSound();
    }

    UpdateShieldCounter();
    isTimerRunning = false;

    // Move to the next question if available, otherwise delay and end quiz
    currentQuestionIndex++;

    if (currentQuestionIndex >= randomizedQuestions.Count)
    {
        // Wait for 2 seconds before showing "Quiz Complete" screen
        Invoke(nameof(QuizComplete), 2f);
    }
    else
    {
        Invoke(nameof(DisplayQuestion), 2f); // Delay before moving to the next question
    }
}




   //    void ShowSymbol(Image symbol)
   //   {
   //       correctSymbol.gameObject.SetActive(false);
   //       wrongSymbol.gameObject.SetActive(false);
//
   //       symbol.gameObject.SetActive(true);
   //       Invoke(nameof(HideSymbols), 1f);  // Delay to hide symbols after 1 second
   //   }
//
   //   void HideSymbols()
   //   {
   //       correctSymbol.gameObject.SetActive(false);
   //       wrongSymbol.gameObject.SetActive(false);
   //   }

    void UpdateShieldCounter()
    {
        string color = (shields > 0) ? "green" : "white"; // Green if shields > 0, otherwise Red

        shieldCounterText.text = 
            
           // $"<color=yellow><b><size=36>" +
           // $"</size></b>🛡</color>" +
            $"<color={color}>{shields} of 3</color>";
            
    }




        

        void LoadNextGameScene()
    {
        PlayerPrefs.SetInt("ShieldCount", shields); // Save the shield count
        PlayerPrefs.Save(); // Ensure the data is written to disk
        SceneManager.LoadScene("level2");   // Load the next game scene
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

        IEnumerator AnimateProcessingText()
{
    string baseText = "Processing Data"; 
    int dotCount = 0;

    while (true)
    {
        // Update the text with increasing dots
        quizCompleteTextObject.GetComponent<TMP_Text>().text = baseText + new string('.', dotCount);

        // Increment dot count (cycle from 0 to 3 dots)
        dotCount = (dotCount + 1) % 4;

        yield return new WaitForSeconds(0.5f); // Adjust speed if needed
    }
}

    }
