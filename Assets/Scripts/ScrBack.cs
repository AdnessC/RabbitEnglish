
using UnityEngine;
using UnityEngine.SceneManagement; // Добавляем для работы со сценами
using UnityEngine.UI; // Добавляем для работы с UI элементами

public class ScrBack : MonoBehaviour
{
    // Добавляем ссылку на кнопку (можно привязать в инспекторе)
    public Button backButton;

    void Start()
    {
        // Проверяем, есть ли кнопка и добавляем обработчик
        if (backButton != null)
        {
            backButton.onClick.AddListener(LoadMainMenu);
        }
        else
        {
            Debug.LogWarning("Кнопка 'Назад' не назначена в инспекторе");
        }
    }

    // Метод для загрузки главного меню (сцена с индексом 0)
    void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    // Если хотите сделать кнопку работающей и через Update (по клавише Escape)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMainMenu();
        }
    }
}