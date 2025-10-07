using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class AnswerChecker : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private TMP_InputField answerInputField;
    [SerializeField] private Animator animator;
    [SerializeField] private string correctTrigger = "Correct";
    [SerializeField] private string wrongTrigger = "Wrong";
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] private AudioSource audioSource;
    private TMP_Text errorText;
    private string _lastShownTextureName;

    private void Start()
    {
        correctSound = Resources.Load<AudioClip>("Audio/Верно!");
        wrongSound = Resources.Load<AudioClip>("Audio/Не верно");

        // Проверяем загрузку
        if (correctSound == null) Debug.LogError("Не найден звук 'Верно!' в папке Resources/Audio");
        if (wrongSound == null) Debug.LogError("Не найден звук 'Не верно' в папке Resources/Audio");
        // Настраиваем поле ввода
        answerInputField.onSubmit.AddListener(HandleAnswerSubmit); // Для Enter
        answerInputField.onEndEdit.AddListener(HandleAnswerSubmit); // Для мобильных "Готово"
    }

    // Вызывается при показе карточки (из CardController)
    public void OnCardShown(Texture2D texture)
    {
        _lastShownTextureName = texture.name;
        answerInputField.text = ""; // Очищаем поле при новой карточке
        Debug.Log($"Показана карточка: {_lastShownTextureName}");
    }

    // Обработчик ввода (вызывается при Enter/Готово)
    private void HandleAnswerSubmit(string userInput)
    {
        // Игнорируем, если поле пустое или потеряло фокус без подтверждения
        if (string.IsNullOrEmpty(userInput)) return;

        CheckAnswer();
    }

    private void CheckAnswer()
    {
        if (string.IsNullOrEmpty(_lastShownTextureName))
        {
            Debug.Log("Ошибка: карточка не была показана");
            errorText.gameObject.SetActive(true);
            StartCoroutine(HideErrorMessageAfterDelay(2f));
            return;
        }

        string correctAnswer = System.IO.Path.GetFileNameWithoutExtension(_lastShownTextureName);
        string userAnswer = answerInputField.text.Trim();

        bool isCorrect = string.Equals(
            userAnswer,
            correctAnswer,
            System.StringComparison.OrdinalIgnoreCase);

        // Анимация + звук
        animator.SetTrigger(isCorrect ? correctTrigger : wrongTrigger);
        StartCoroutine(PlaySoundWithDelay(isCorrect));
        answerInputField.text = "";

        Debug.Log($"Проверено: {userAnswer} = {correctAnswer} → {isCorrect}");
    }
    private IEnumerator PlaySoundWithDelay(bool isCorrect)
    {
        yield return new WaitForSeconds(0.1f); // Ждём 
        PlayFeedbackSound(isCorrect);
    }
    private void PlayFeedbackSound(bool isCorrect)
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        AudioClip clip = isCorrect ? correctSound : wrongSound;
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    private IEnumerator HideErrorMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        errorText.gameObject.SetActive(false);
    }
}