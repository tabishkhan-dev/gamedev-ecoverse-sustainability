using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

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

    // Audio Elements
    public AudioClip quizBackgroundMusic; // Background music for the quiz
    public AudioClip correctAnswerSound;  // Sound effect for correct answers
    public AudioClip wrongAnswerSound;    // Sound effect for wrong answers

    private List<QuizData.Question> randomizedQuestions;
    private int currentQuestionIndex = 0;
    private float timePerQuestion = 15f;
    private float timeRemaining;
    private bool isTimerRunning = false;
    private int shields = 0;
    private AudioSource audioSource;

    public string nextGameSceneName = "GameLevel"; // Name of the game scene to return to

    void Start()
    {
        InitializeQuiz();
        submitButton.onClick.AddListener(CheckAnswer);
        proceedButton.onClick.AddListener(LoadNextGameScene);

        // Set initial button text
        // Set button text
        if (submitButtonText != null)
            submitButtonText.text = "Submit";
    
        if (proceedButtonText != null)
            proceedButtonText.text = "Proceed";

        proceedButton.gameObject.SetActive(false);  // Hide proceed button initially

        audioSource = gameObject.AddComponent<AudioSource>();
        PlayBackgroundMusic();
    }

    void InitializeQuiz()
    {
        randomizedQuestions = new List<QuizData.Question>(quizData.questions);
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

        randomizedQuestions = randomizedQuestions.GetRange(0, Mathf.Min(3, randomizedQuestions.Count));
    }

    void DisplayQuestion()
    {
        // Hide the symbols when moving to the next question
        correctSymbol.gameObject.SetActive(false);
        wrongSymbol.gameObject.SetActive(false);
    
        if (currentQuestionIndex < randomizedQuestions.Count)
        {
            questionText.text = randomizedQuestions[currentQuestionIndex].questionText;
            questionText.rectTransform.sizeDelta = new Vector2(0, questionText.rectTransform.sizeDelta.y); // Stretch across width
            feedbackText.text = "";
            answerInput.text = "";
    
            // Restart the timer
            timeRemaining = timePerQuestion;
            isTimerRunning = true;
            StartCoroutine(UpdateTimer());
        }
        else
        {
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

    void LoadNextGameScene()
    {
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
} 