using UnityEngine;

public class WinBoll : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        // ���������, ���� �� ������� �� ������ ��� ������� ����
        if (Input.touchCount > 0)
        {
            Debug.Log("����� ��� ����� ��� ���� ������!"); // ���������� ���������
            // �������� ������, �������� ���
            gameObject.SetActive(false);
        }
    }
}