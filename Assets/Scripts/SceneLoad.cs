using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    public GameObject InterFace;

    void Start()
    {
        // �������� ������� ������ �����
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // �������� ����� ���������� ���� � ������
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        // ��������� �������� ����� � �������� 1
        if (currentSceneIndex == 1)
        {
            if (InterFace != null)
            {
                InterFace.SetActive(true); // ���������� ������, ���� ����� ���������
            }
        }
        else
        {
            if (InterFace != null)
            {
                InterFace.SetActive(false); // �������� ������, ���� ����� �� ���������
            }
        }
    }
}
