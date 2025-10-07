using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class AnimationSwitcherNew : MonoBehaviour
{
    public Animator animator; // Ссылка на компонент Animator
    public TMP_Text errorText;
    public string breathAnimation = "Дыхание"; // Имя анимации дыхания
    public string yawnAnimationCheck = "Check"; // Имя триггера для смены анимации на зевание
    public string QashAnimationTrigger = "Question"; // Имя триггера для анимации вопроса

    [SerializeField] private AudioSource _audioSource;
    public Button[] controlButtons; // Кнопки, которые нужно отключать
    private Dictionary<string, List<Texture2D>> _allCards;
    private JsonDataManager _dataManager;
    private string _currentUserId;
    private Button yourButton;
    private bool _isYawning = false; // Флаг для отслеживания анимации зевания
    private Coroutine _breathCoroutine;
    private bool _isPlayingQuestAnimation = false;

    void Start()
    {
        // Проверяем, что это сцена с индексом 1
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            StartCoroutine(PlayBreathAnimation());
        }
        else
        {
            // Опционально: отключаем аниматор на других сценах
            if (animator != null)
            {
                animator.enabled = false;
            }
        }

    }

    private IEnumerator PlayBreathAnimation()
    {
        // Заранее загружаем аудио (делаем это один раз)
         AudioClip yawnSound = Resources.Load<AudioClip>("Audio/Зевание");

        while (true)
        {
            // Если идет другая анимация - ждем
            if (_isYawning || _isPlayingQuestAnimation)
            {
                yield return null;
                continue;
            }

            // Проигрываем цикл дыхания
            for (int i = 0; i < 20; i++)
            {
                if (_isPlayingQuestAnimation) break;

                animator.Play(breathAnimation);
                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            }

            // Проигрываем зевание, если не было прерывания
            if (!_isPlayingQuestAnimation && !_isYawning)
            {
                _isYawning = true;
                animator.SetTrigger(yawnAnimationCheck);

                // Воспроизводим звук (если он загрузился)
                if (_audioSource != null && yawnSound != null)
                {
                    _audioSource.PlayOneShot(yawnSound); 
                }

                yield return new WaitForSeconds(6.75f); // Полная длительность зевания
                _isYawning = false;
            }
        }
    }

    void OnEnable()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        _breathCoroutine = StartCoroutine(PlayBreathAnimation());
        UpdateButtonsInteractable();
    }

    void OnDisable()
    {
        if (_breathCoroutine != null)
            StopCoroutine(_breathCoroutine);
    }

    // Метод для запуска новой анимации
    public void OnAnimationQuest()
    {
        if (_isPlayingQuestAnimation) return;

        _isPlayingQuestAnimation = true;
        UpdateButtonsInteractable();

        animator.SetTrigger(QashAnimationTrigger);
        StartCoroutine(PlayAnimationWithPause());
    }

    private IEnumerator PlayAnimationWithPause()
    {
        // Ждём 1.16 секунды анимации
        yield return new WaitForSeconds(1.3f);

        // Ставим анимацию на паузу (speed = 0)
        animator.speed = 0f;

        // Ждём 2 секунды паузы
        yield return new WaitForSeconds(1.5f);

        // Возобновляем анимацию (speed = 1)
        animator.speed = 1f;

        // Ждём оставшееся время (2.5 - 1.16 = 1.34 секунды)
        yield return new WaitForSeconds(1.34f);

        // Включаем кнопки и сбрасываем флаг
        _isPlayingQuestAnimation = false;
        UpdateButtonsInteractable();
    }

    private void UpdateButtonsInteractable()
    {
        bool canInteract = !_isPlayingQuestAnimation;
        foreach (Button button in controlButtons)
        {
            if (button != null)
                button.interactable = canInteract;
        }
    }

    void Update()
    {
        Debug.Log($"State: {(_isYawning ? "Yawning" : _isPlayingQuestAnimation ? "QuestAnimation" : "Breathing")}");
    }

    private IEnumerator SmoothAnimationWithPause()
    {
        // Ждём, пока анимация реально начнётся
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0);

        // Точно ждём 1.3 секунды анимации
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        float elapsed = 0f;

        while (elapsed < 1.3f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Пауза 1.5 секунды (speed = 0)
        animator.speed = 0f;
        yield return new WaitForSeconds(1.5f);
        animator.speed = 1f;

        // Ждём **оставшуюся** часть анимации (без лишних секунд!)
        float remainingTime = animLength - 1.3f; // Общая длина минус уже проигранное время
        yield return new WaitForSeconds(remainingTime);

        // Сброс состояния
        _isPlayingQuestAnimation = false;
        UpdateButtonsInteractable();
    }


    public void OnAnimationQuestR(string category)
    {
        if (_isPlayingQuestAnimation) return;

        CardController cardController = FindObjectOfType<CardController>();
        if (cardController == null) return;

        cardController.ShowRandomCard(category);

        if (cardController.card.activeInHierarchy)
        {
            _isPlayingQuestAnimation = true;
            UpdateButtonsInteractable();

            animator.SetTrigger(QashAnimationTrigger);
            StartCoroutine(SmoothAnimationWithPause());
        }
        else
        {
            if (errorText != null)
            {
                errorText.gameObject.SetActive(true);
                StartCoroutine(HideErrorMessageAfterDelay(2f));
            }
        }
    }

    private IEnumerator HideErrorMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        errorText.gameObject.SetActive(false);
    }

    public void PlayAnimalsAnimation() => OnAnimationQuestR("Animals");
    public void PlayVerbsAnimation() => OnAnimationQuestR("Verbs");
    public void PlayAbjectivesAnimation() => OnAnimationQuestR("Abjectives");
    public void PlayColorAnimation() => OnAnimationQuestR("Colors");
    public void PlayPeopleAnimation() => OnAnimationQuestR("People");
    public void PlayObjAnimation() => OnAnimationQuestR("Objects");

}

