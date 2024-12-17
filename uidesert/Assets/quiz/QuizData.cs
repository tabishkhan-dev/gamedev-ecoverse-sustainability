using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuizData", menuName = "Quiz/QuizData")]
public class QuizData : ScriptableObject
{
    [System.Serializable]
    public class Question
    {
        public string questionText; // The question text
        public List<string> correctAnswers; // List of acceptable answers // The correct answer
    }

    public Question[] questions; // Array of questions
}

