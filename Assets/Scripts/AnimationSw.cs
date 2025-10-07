using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class AnimationSwitcherNew : MonoBehaviour
{
    public Animator animator; // ������ �� ��������� Animator
    public TMP_Text errorText;
    public string breathAnimation = "�������"; // ��� �������� �������
    public string yawnAnimationCheck = "Check"; // ��� �������� ��� ����� �������� �� �������
    public string QashAnimationTrigger = "Question"; // ��� �������� ��� �������� �������

    [SerializeField] private AudioSource _audioSource;
    public Button[] controlButtons; // ������, ������� ����� ���������
    private Dictionary<string, List<Texture2D>> _allCards;
    private JsonDataManager _dataManager;
    private string _currentUserId;
    private Button yourButton;
    private bool _isYawning = false; // ���� ��� ������������ �������� �������
    private Coroutine _breathCoroutine;
    private bool _isPlayingQuestAnimation = false;

    void Start()
    {
        // ���������, ��� ��� ����� � �������� 1
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            StartCoroutine(PlayBreathAnimation());
        }
        else
        {
            // �����������: ��������� �������� �� ������ ������
            if (animator != null)
            {
                animator.enabled = false;
            }
        }

    }

    private IEnumerator PlayBreathAnimation()
    {
        // ������� ��������� ����� (������ ��� ���� ���)
         AudioClip yawnSound = Resources.Load<AudioClip>("Audio/�������");

        while (true)
        {
            // ���� ���� ������ �������� - ����
            if (_isYawning || _isPlayingQuestAnimation)
            {
                yield return null;
                continue;
            }

            // ����������� ���� �������
            for (int i = 0; i < 20; i++)
            {
                if (_isPlayingQuestAnimation) break;

                animator.Play(breathAnimation);
                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            }

            // ����������� �������, ���� �� ���� ����������
            if (!_isPlayingQuestAnimation && !_isYawning)
            {
                _isYawning = true;
                animator.SetTrigger(yawnAnimationCheck);

                // ������������� ���� (���� �� ����������)
                if (_audioSource != null && yawnSound != null)
                {
                    _audioSource.PlayOneShot(yawnSound); 
                }

                yield return new WaitForSeconds(6.75f); // ������ ������������ �������
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

    // ����� ��� ������� ����� ��������
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
        // ��� 1.16 ������� ��������
        yield return new WaitForSeconds(1.3f);

        // ������ �������� �� ����� (speed = 0)
        animator.speed = 0f;

        // ��� 2 ������� �����
        yield return new WaitForSeconds(1.5f);

        // ������������ �������� (speed = 1)
        animator.speed = 1f;

        // ��� ���������� ����� (2.5 - 1.16 = 1.34 �������)
        yield return new WaitForSeconds(1.34f);

        // �������� ������ � ���������� ����
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
        // ���, ���� �������� ������� �������
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0);

        // ����� ��� 1.3 ������� ��������
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        float elapsed = 0f;

        while (elapsed < 1.3f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ����� 1.5 ������� (speed = 0)
        animator.speed = 0f;
        yield return new WaitForSeconds(1.5f);
        animator.speed = 1f;

        // ��� **����������** ����� �������� (��� ������ ������!)
        float remainingTime = animLength - 1.3f; // ����� ����� ����� ��� ����������� �����
        yield return new WaitForSeconds(remainingTime);

        // ����� ���������
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

