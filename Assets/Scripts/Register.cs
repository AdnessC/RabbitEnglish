using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Security.Cryptography;

public class Register : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button registerButton;
    public TMP_Text statusText;

    private JsonDataManager _dataManager;

    void Start()
    {
        _dataManager = FindObjectOfType<JsonDataManager>();
        registerButton.onClick.AddListener(OnRegisterButtonClick);
    }

    public void OnRegisterButtonClick()
    {
        statusText.text = "";

        if (_dataManager == null)
        {
            _dataManager = FindObjectOfType<JsonDataManager>();
            if (_dataManager == null)
            {
                ShowError("Системная ошибка!");
                return;
            }
        }

        if (string.IsNullOrEmpty(usernameInput.text))
        {
            ShowError("Введите логин!");
            return;
        }

        if (string.IsNullOrEmpty(passwordInput.text))
        {
            ShowError("Введите пароль!");
            return;
        }

        try
        {
            // Хешируем пароль перед сохранением
            byte[] salt = GenerateSalt();
            string passwordHash = HashPasswordPBKDF2(passwordInput.text, salt);

            // Создаем нового пользователя
            UserData newUser = _dataManager.CreateNewUser();
            newUser.username = usernameInput.text;
            newUser.password = passwordHash;
            newUser.salt = Convert.ToBase64String(salt);

            // Инициализируем прогресс
            newUser.learningProgress = new Dictionary<string, int>
        {
            {"Animals", 0},
            {"Verbs", 0},
            {"Abjectives", 0},
            {"Colors", 0},
            {"People", 0},
            {"Objects", 0},
        };

            _dataManager.SaveUserData(newUser);
            PlayerPrefs.SetString("CurrentUserCode", newUser.userCode);

            ShowSuccess($"Регистрация успешна! Код: {newUser.userCode}");
            StartCoroutine(LoadSceneAfterDelay(1, 1.5f));
        }
        catch (Exception e)
        {
            ShowError($"Ошибка регистрации: {e.Message}");
        }
    }
    // --- Методы для хеширования PBKDF2 ---
    private byte[] GenerateSalt()
    {
        byte[] salt = new byte[16]; // 128-битная соль
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    private string HashPasswordPBKDF2(string password, byte[] salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(
            password: password,
            salt: salt,
            iterations: 100000, // Рекомендуемое число итераций
            hashAlgorithm: HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(32); // 256-битный хеш
            return Convert.ToBase64String(hash);
        }
    }
    private IEnumerator LoadSceneAfterDelay(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneIndex);
    }

    private void ShowSuccess(string message)
    {
        statusText.text = message;
        statusText.color = Color.green;
    }

    private void ShowError(string message)
    {
        statusText.text = message;
        statusText.color = Color.red;
    }

    private IEnumerator HideStatusAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        statusText.text = "";
    }

    void OnDestroy()
    {
        registerButton.onClick.RemoveListener(OnRegisterButtonClick);
    }
}
