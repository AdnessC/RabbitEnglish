using System;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text statusText;

    private JsonDataManager _dataManager;

    void Awake()
    {
        // ��������� ������ � ����������
        if (usernameInput == null || passwordInput == null ||
            loginButton == null || statusText == null)
        {
            Debug.LogError("�� ��� UI �������� ��������� � ����������!");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        _dataManager = FindObjectOfType<JsonDataManager>();

        if (_dataManager == null)
        {
            Debug.LogError("JsonDataManager �� ������ �� �����!");
            statusText.text = "������ �������! ������������� ����������.";
            statusText.color = Color.red;
            loginButton.interactable = false;
            return;
        }

        loginButton.onClick.AddListener(OnLoginButtonClick);
    }

    public void OnLoginButtonClick()
    {
        if (_dataManager == null)
        {
            _dataManager = FindObjectOfType<JsonDataManager>();
            if (_dataManager == null)
            {
                statusText.text = "��������� ������!";
                return;
            }
        }

        statusText.text = "";
        statusText.color = Color.red;

        // ��������� ����
        if (string.IsNullOrEmpty(usernameInput.text))
        {
            statusText.text = "������� �����!";
            return;
        }

        if (string.IsNullOrEmpty(passwordInput.text))
        {
            statusText.text = "������� ������!";
            return;
        }

        // ������� �����
        try
        {
            var allUserCodes = _dataManager.GetAllUserCodes();
            foreach (var code in allUserCodes)
            {
                var userData = _dataManager.LoadUserData(code);
                if (userData != null && userData.username == usernameInput.text.Trim())
                {
                    // ��������� ������ ����� PBKDF2
                    byte[] salt = Convert.FromBase64String(userData.salt);
                    string inputHash = Convert.ToBase64String(
                        new System.Security.Cryptography.Rfc2898DeriveBytes(
                            passwordInput.text.Trim(),
                            salt,
                            100000,
                            System.Security.Cryptography.HashAlgorithmName.SHA256)
                        .GetBytes(32));

                    if (inputHash == userData.password)
                    {
                        PlayerPrefs.SetString("CurrentUserCode", code);
                        SceneManager.LoadScene(1);
                        return;
                    }
                }
            }
            statusText.text = "�������� ����� ��� ������!";
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            statusText.text = "������ �������!";
        }
    }

    void OnDestroy()
    {
        if (loginButton != null)
        {
            loginButton.onClick.RemoveListener(OnLoginButtonClick);
        }
    }
}