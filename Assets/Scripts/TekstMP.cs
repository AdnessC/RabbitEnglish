using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TekstMP : MonoBehaviour
{
    [Header("���������")]
    [SerializeField] private TMP_Text _outputText; // ���� �������� �����
    [SerializeField] private List<Button> _buttons = new List<Button>(); // ���� �������������� ������ ������
    [SerializeField] private bool _startHidden = true; // �������� �� ������� �������

    [Header("��������")]
    [SerializeField] private float _animDuration = 2f; // ����� ������������ ��������
    [SerializeField] private float _popScale = 10f; // �� ������� ��� ������������� �����
    [SerializeField] private Vector2 _centerPosition = new Vector2(0, 0); // ����� ������

    private Vector3 _originalPosition;
    private Vector3 _originalScale;
    private Coroutine _currentAnimation;

    private void Start()
    {
        _originalPosition = _outputText.rectTransform.localPosition;
        _originalScale = _outputText.rectTransform.localScale;

        // ��������� ���������
        if (_startHidden)
        {
            _outputText.gameObject.SetActive(false);
        }

        foreach (Button button in _buttons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText == null) continue;

            button.onClick.AddListener(() => OnButtonClicked(button));
        }
    }

    private void OnButtonClicked(Button clickedButton)
    {
        TMP_Text buttonText = clickedButton.GetComponentInChildren<TMP_Text>();
        if (_outputText == null || buttonText == null) return;

        // ���������� ������, ���� �� ��� �����
        if (!_outputText.gameObject.activeSelf)
        {
            _outputText.gameObject.SetActive(true);
        }

        _outputText.text = buttonText.text;

        if (_currentAnimation != null)
        {
            StopCoroutine(_currentAnimation);
        }

        _currentAnimation = StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        // 1. ������������� ��������� ���������
        _outputText.rectTransform.localPosition = _centerPosition;
        _outputText.rectTransform.localScale = Vector3.zero;
        _outputText.alpha = 1f;

        // 2. �������� ���������� (���������)
        float timer = 0f;
        while (timer < _animDuration / 2)
        {
            timer += Time.deltaTime;
            float progress = timer / (_animDuration / 2);

            _outputText.rectTransform.localScale = Vector3.Lerp(
                Vector3.zero,
                _originalScale * _popScale,
                progress
            );

            yield return null;
        }

        // 3. �������� ��������
        timer = 0f;
        Vector3 startScale = _outputText.rectTransform.localScale;
        Vector3 startPos = _outputText.rectTransform.localPosition;

        while (timer < _animDuration / 2)
        {
            timer += Time.deltaTime;
            float progress = timer / (_animDuration / 2);

            _outputText.rectTransform.localScale = Vector3.Lerp(
                startScale,
                _originalScale,
                progress
            );

            _outputText.rectTransform.localPosition = Vector3.Lerp(
                startPos,
                _originalPosition,
                progress
            );

            yield return null;
        }

        // 4. ��������� �������� ��������
        _outputText.rectTransform.localScale = _originalScale;
        _outputText.rectTransform.localPosition = _originalPosition;
    }

    private void OnDestroy()
    {
        foreach (Button button in _buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}