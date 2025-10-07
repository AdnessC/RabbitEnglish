using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using System.IO; // Для проверки файлов

public class GameSceneController : MonoBehaviour
{
    [SerializeField] private TMP_Text uidText;
    [SerializeField] private CardController cardController;
    [SerializeField] private Button targetButton; // Кнопка, которую будем отключать

    void Start()
    {
        string userCode = PlayerPrefs.GetString("CurrentUserCode", "неизвестен");

        // Отображаем UID
        if (uidText != null)
        {
            uidText.text = $"UID: {userCode}";
        }

        // Проверяем условия и блокируем кнопку
        if (targetButton != null)
        {
            bool shouldDisableButton =
                string.IsNullOrEmpty(userCode) ||
                userCode == "неизвестен" ||
                !UserJsonFileExists(userCode);

            targetButton.interactable = !shouldDisableButton;
        }
    }

    // Проверяем существование файла пользователя
    private bool UserJsonFileExists(string userCode)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"user_{userCode}.json");
        return File.Exists(filePath);
    }
}
