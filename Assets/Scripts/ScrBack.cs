
using UnityEngine;
using UnityEngine.SceneManagement; // ��������� ��� ������ �� �������
using UnityEngine.UI; // ��������� ��� ������ � UI ����������

public class ScrBack : MonoBehaviour
{
    // ��������� ������ �� ������ (����� ��������� � ����������)
    public Button backButton;

    void Start()
    {
        // ���������, ���� �� ������ � ��������� ����������
        if (backButton != null)
        {
            backButton.onClick.AddListener(LoadMainMenu);
        }
        else
        {
            Debug.LogWarning("������ '�����' �� ��������� � ����������");
        }
    }

    // ����� ��� �������� �������� ���� (����� � �������� 0)
    void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    // ���� ������ ������� ������ ���������� � ����� Update (�� ������� Escape)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMainMenu();
        }
    }
}