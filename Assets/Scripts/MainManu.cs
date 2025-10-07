using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManu : MonoBehaviour
{
  public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Debug.Log("Игра закрытв");
        Application.Quit();
    }
//Скрипт написанный только для того, чтобы проверять как закрывается игра через кнопку и закрывается ли вообще
