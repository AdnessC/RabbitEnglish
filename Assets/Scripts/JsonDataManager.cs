using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class JsonDataManager : MonoBehaviour
{
    private string _directoryPath;
    private const string UserDataFilePrefix = "user_";

    public static JsonDataManager Instance { get; private set; }

    void Awake()
    {
        _directoryPath = Application.persistentDataPath;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDirectory(); // Добавляем инициализацию директории
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDirectory()
    {
        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }
    }

    private string GenerateUserCode()
    {
        int maxCode = 9999;
        var files = Directory.GetFiles(_directoryPath, $"{UserDataFilePrefix}*.json");

        foreach (var file in files)
        {
            string json = File.ReadAllText(file);
            var data = JsonUtility.FromJson<UserData>(json);

            if (int.TryParse(data.userCode, out int code) && code > maxCode)
            {
                maxCode = code;
            }
        }

        return (maxCode + 1).ToString();
    }

    public UserData CreateNewUser()
    {
        var newUser = new UserData();
        newUser.userCode = GenerateUserCode();
        SaveUserData(newUser);
        return newUser;
    }

    public void SaveUserData(UserData userData)
    {
        userData.PrepareToSave(); // Подготавливаем данные к сохранению

        string json = JsonUtility.ToJson(userData, true);
        string filePath = Path.Combine(Application.persistentDataPath, $"user_{userData.userCode}.json");
        File.WriteAllText(filePath, json);
    }

    public UserData LoadUserData(string userCode)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"user_{userCode}.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            UserData userData = JsonUtility.FromJson<UserData>(json);
            userData.AfterLoad(); // Восстанавливаем словари после загрузки
            return userData;
        }

        // Если файла нет, создаем нового пользователя
        return new UserData() { userCode = userCode };
    }

    public bool ValidateCredentials(string userCode, string username, string password)
    {
        var userData = LoadUserData(userCode);
        return userData != null && userData.username == username && userData.password == password;
    }

    public List<string> GetAllUserCodes()
    {
        List<string> userCodes = new List<string>();
        var files = Directory.GetFiles(_directoryPath, $"{UserDataFilePrefix}*.json");

        foreach (var file in files)
        {
            string json = File.ReadAllText(file);
            var data = JsonUtility.FromJson<UserData>(json);
            userCodes.Add(data.userCode);
        }

        return userCodes;
    }
}