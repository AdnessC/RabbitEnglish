using System.Collections.Generic;

[System.Serializable]
public class UserData
{
    public string username;
    public string password;
    public string salt;
    public string userCode;
    public List<string> viewedImages = new List<string>();

    // Добавляем новые поля для прогресса изучения
    public Dictionary<string, int> learningProgress = new Dictionary<string, int>();

    // Для сериализации Dictionary
    public List<string> progressCategories = new List<string>();
    public List<int> progressIndices = new List<int>();

    public void PrepareToSave()
    {
        progressCategories.Clear();
        progressIndices.Clear();

        foreach (var kvp in learningProgress)
        {
            progressCategories.Add(kvp.Key);
            progressIndices.Add(kvp.Value);
        }
    }

    public void AfterLoad()
    {
        learningProgress.Clear();

        for (int i = 0; i < progressCategories.Count; i++)
        {
            learningProgress[progressCategories[i]] = progressIndices[i];
        }
    }
}
}
