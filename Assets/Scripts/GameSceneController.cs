using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using System.IO; // ��� �������� ������

public class GameSceneController : MonoBehaviour
{
    [SerializeField] private TMP_Text uidText;
    [SerializeField] private CardController cardController;
    [SerializeField] private Button targetButton; // ������, ������� ����� ���������

    void Start()
    {
        string userCode = PlayerPrefs.GetString("CurrentUserCode", "����������");

        // ���������� UID
        if (uidText != null)
        {
            uidText.text = $"UID: {userCode}";
        }

        // ��������� ������� � ��������� ������
        if (targetButton != null)
        {
            bool shouldDisableButton =
                string.IsNullOrEmpty(userCode) ||
                userCode == "����������" ||
                !UserJsonFileExists(userCode);

            targetButton.interactable = !shouldDisableButton;
        }
    }

    // ��������� ������������� ����� ������������
    private bool UserJsonFileExists(string userCode)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"user_{userCode}.json");
        return File.Exists(filePath);
    }
}