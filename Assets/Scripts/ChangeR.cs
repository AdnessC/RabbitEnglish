using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
    [Header("3D ������")]
    [SerializeField] private Renderer targetRenderer; // �������� �������, ������� ������

    [Header("�����")]
    [SerializeField] private Color defaultColor = new Color(0.906f, 0.996f, 0.6f); // E7FE99
    [SerializeField] private Color alternateColor = new Color(0.984f, 0.816f, 0.431f); // FBD06E
    [SerializeField] private Color noUserColor = new Color(0.580f, 0.588f, 0.494f); // 94967E

    [Header("������")]
    [SerializeField] private Button changeColorButton; // ������ ��� ����� �����
    [SerializeField] private Button resetColorButton; // ������ ��� ������ �����

    [Header("����� �����������")]
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private float notificationDuration = 1f;
    [SerializeField] private float notificationScale = 2f;
    [SerializeField] private Vector2 notificationPosition = Vector2.zero;

    private Material objectMaterial;
    private bool hasValidUser = false;
    private Vector3 originalTextPosition;
    private Vector3 originalTextScale;
    private Coroutine currentNotification;

    private void Start()
    {
        // ��������� ������� ������������
        CheckUserStatus();

        // ������������� ���������
        objectMaterial = targetRenderer.material;
        ApplyDefaultColor();

        // ��������� ������������ ��������� ������
        if (notificationText != null)
        {
            originalTextPosition = notificationText.rectTransform.localPosition;
            originalTextScale = notificationText.rectTransform.localScale;
            notificationText.gameObject.SetActive(false);
        }

        // ��������� ������
        if (changeColorButton != null)
        {
            changeColorButton.onClick.AddListener(() =>
            {
                ChangeToAlternateColor();
                ShowNotification("����� ����������");
            });
        }

        if (resetColorButton != null)
        {
            resetColorButton.onClick.AddListener(() =>
            {
                ApplyDefaultColor();
                ShowNotification(hasValidUser ? "����� ��������" : "�������� �����");
            });
        }
    }

    private void CheckUserStatus()
    {
        string userCode = PlayerPrefs.GetString("CurrentUserCode", "����������");
        hasValidUser = !string.IsNullOrEmpty(userCode) &&
                      userCode != "����������" &&
                      UserJsonFileExists(userCode);
    }

    private bool UserJsonFileExists(string userCode)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"user_{userCode}.json");
        return File.Exists(filePath);
    }

    private void ApplyDefaultColor()
    {
        objectMaterial.color = hasValidUser ? defaultColor : noUserColor;
    }

    private void ChangeToAlternateColor()
    {
        if (hasValidUser)
        {
            objectMaterial.color = alternateColor;
        }
    }

    private void ShowNotification(string message)
    {
        if (notificationText == null) return;

        if (currentNotification != null)
        {
            StopCoroutine(currentNotification);
        }

        currentNotification = StartCoroutine(ShowNotificationCoroutine(message));
    }

    private IEnumerator ShowNotificationCoroutine(string message)
    {
        // ������������� ����� � ���������
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        notificationText.rectTransform.localPosition = notificationPosition;
        notificationText.rectTransform.localScale = originalTextScale * notificationScale;

        // ���� ��������� �����
        yield return new WaitForSeconds(notificationDuration);

        // ���������� � �������� ���������
        notificationText.rectTransform.localPosition = originalTextPosition;
        notificationText.rectTransform.localScale = originalTextScale;
        notificationText.gameObject.SetActive(false);

        currentNotification = null;
    }

    private void OnDestroy()
    {
        if (changeColorButton != null)
        {
            changeColorButton.onClick.RemoveAllListeners();
        }

        if (resetColorButton != null)
        {
            resetColorButton.onClick.RemoveAllListeners();
        }
    }
}