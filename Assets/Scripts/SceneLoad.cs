using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    public GameObject InterFace;

    void Start()
    {
        // Получаем текущий индекс сцены
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // Получаем общее количество сцен в сборке
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        // Проверяем загрузку сцены с индексом 1
        if (currentSceneIndex == 1)
        {
            if (InterFace != null)
            {
                InterFace.SetActive(true); // Показываем объект, если сцена загружена
            }
        }
        else
        {
            if (InterFace != null)
            {
                InterFace.SetActive(false); // Скрываем объект, если сцена не загружена
            }
        }
    }
}
