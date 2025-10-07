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
        Debug.Log("Игра Закрыта");
        Application.Quit();
    }
}
