using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] answers = new string[4];
    public string correctAnswer;
}

public class CanvasQusetion : MonoBehaviour
{
    public Text questionText;
    public Button[] answerButtons;
    public Question[] questions;
    public GameObject resultUI;
    public Text resultText;
    public string correctMessage = "回答正确";
    public string wrongMessage = "回答错误";

    private int currentIndex;
    private string correctAnswer;

    void Start()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }

        if (resultUI != null)
        {
            resultUI.SetActive(false);
        }

        currentIndex = 0;
        ShowQuestion(currentIndex);
    }

    void ShowQuestion(int index)
    {
        if (index < 0 || index >= questions.Length) return;

        Question q = questions[index];

        if (questionText != null)
        {
            questionText.text = q.questionText;
        }

        correctAnswer = q.correctAnswer;

        for (int i = 0; i < answerButtons.Length && i < q.answers.Length; i++)
        {
            Text buttonText = answerButtons[i].GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = q.answers[i];
            }
        }
    }

    void CheckAnswer(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= answerButtons.Length) return;

        Text buttonText = answerButtons[buttonIndex].GetComponentInChildren<Text>();
        string selectedAnswer = buttonText != null ? buttonText.text : "";

        bool isCorrect = selectedAnswer == correctAnswer;

        if (resultUI != null)
        {
            resultUI.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = isCorrect ? correctMessage : wrongMessage;
        }
    }

    public void NextQuestion()
    {
        currentIndex++;
        if (currentIndex >= questions.Length)
        {
            currentIndex = 0;
        }

        if (resultUI != null)
        {
            resultUI.SetActive(false);
        }

        ShowQuestion(currentIndex);
    }

    public void HideCanvas()
    {
        gameObject.SetActive(false);
    }

    public void ShowCanvas()
    {
        gameObject.SetActive(true);
    }
}
