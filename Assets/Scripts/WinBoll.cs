using UnityEngine;

public class WinBoll : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        // Проверяем, есть ли касание на экране или нажатие мыши
        if (Input.touchCount > 0)
        {
            Debug.Log("Экран был нажат или мышь нажата!"); // Отладочное сообщение
            // Скрываем объект, отключая его
            gameObject.SetActive(false);
        }
    }
}