using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
public class CardController : MonoBehaviour
{
    // Основные настройки
    public GameObject card;
    public string imagesFolder1 = "Image/Animals";
    public string imagesFolder2 = "Image/Verbs";
    public string imagesFolder3 = "Image/Abjectives";
    public string imagesFolder4 = "Image/Colors";
    public string imagesFolder5 = "Image/People";
    public string imagesFolder6 = "Image/Objects";
    public Slider progressSlider;

    // Приватные переменные
    private Dictionary<string, List<Texture2D>> _allCards = new Dictionary<string, List<Texture2D>>();
    private Dictionary<string, int> _currentIndex = new Dictionary<string, int>();
    private bool _isAnimating;
    private string _currentCategory;
    private JsonDataManager _dataManager;
    private string _currentUserId;
    private bool _isGuestMode = false;

    void Awake()
    {
        // Инициализация словарей
        _allCards = new Dictionary<string, List<Texture2D>>();
        _currentIndex = new Dictionary<string, int>()
        {
            {"Animals", 0},
            {"Verbs", 0},
            {"Abjectives", 0},
            {"Colors", 0},
            {"People", 0},
            {"Objects", 0}
        };
    }

    void Start()
    {
        card.SetActive(false);
        InitializeSystem();
    }

    private void InitializeSystem()
    {
        // Получаем DataManager
        _dataManager = JsonDataManager.Instance;
        if (_dataManager == null)
        {
            Debug.LogError("JsonDataManager не найден!");
            return;
        }

        // Загружаем ID текущего пользователя
        _currentUserId = PlayerPrefs.GetString("CurrentUserCode");
        if (string.IsNullOrEmpty(_currentUserId))
        {
            _isGuestMode = true;
            _currentUserId = "guest_" + System.DateTime.Now.Ticks.ToString(); // Временный ID
            Debug.Log("Гостевой режим активирован");
        }

        // Инициализируем карточки
        InitializeCategories();

        if (!_isGuestMode)
        {
            LoadUserProgress();
        }
        else
        {
            // Сбрасываем прогресс для гостя
            foreach (var category in _currentIndex.Keys.ToList())
            {
                _currentIndex[category] = 0;
            }
        }
    }

    private void InitializeCategories()
    {
        string[] categories = { "Animals", "Verbs", "Abjectives","Colors","People","Objects" };
        string[] folders = { imagesFolder1, imagesFolder2, imagesFolder3, imagesFolder4, imagesFolder5, imagesFolder6 };

        for (int i = 0; i < categories.Length; i++)
        {
            string category = categories[i];
            _allCards[category] = new List<Texture2D>();

            // Загрузка текстур
            Texture2D[] textures = Resources.LoadAll<Texture2D>(folders[i]);
            if (textures != null && textures.Length > 0)
            {
                _allCards[category].AddRange(textures);
                Debug.Log($"Загружено {textures.Length} карточек для категории {category}");
            }
            else
            {
                Debug.LogWarning($"Не найдены текстуры в папке: {folders[i]}");
            }
        }
    }

    private void LoadUserProgress()
    {
        // Загружаем данные пользователя
        UserData userData = _dataManager.LoadUserData(_currentUserId);

        if (userData == null)
        {
            Debug.LogWarning($"Данные пользователя {_currentUserId} не найдены, создаем новые");
            userData = new UserData
            {
                userCode = _currentUserId,
            learningProgress = new Dictionary<string, int>()
            {
                {"Animals", 0},
                {"Verbs", 0},
                {"Adjectives", 0},
                {"Colors", 0},
                {"People", 0},
                {"Objects", 0}
            }
            };
            _dataManager.SaveUserData(userData);
        }

        // Инициализируем прогресс, если его нет
        if (userData.learningProgress == null)
        {
            userData.learningProgress = new Dictionary<string, int>();
            foreach (var category in _currentIndex.Keys)
            {
                userData.learningProgress[category] = 0;
            }
            _dataManager.SaveUserData(userData);
        }

        // Обновляем текущий прогресс
        foreach (var category in _currentIndex.Keys.ToList())
        {
            if (userData.learningProgress != null && userData.learningProgress.ContainsKey(category))
            {
                _currentIndex[category] = userData.learningProgress[category];
            }
            else
            {
                _currentIndex[category] = 0;
            }
        }
    }

    private void SaveUserProgress()
    {
        if (_isGuestMode) return;// Не сохраняем для гостей

        UserData userData = _dataManager.LoadUserData(_currentUserId) ?? new UserData();
        userData.userCode = _currentUserId;

        // Обновляем прогресс
        foreach (var category in _currentIndex.Keys)
        {
            if (userData.learningProgress.ContainsKey(category))
            {
                userData.learningProgress[category] = _currentIndex[category];
            }
            else
            {
                userData.learningProgress.Add(category, _currentIndex[category]);
            }
        }

        // Сохраняем данные
        _dataManager.SaveUserData(userData);
    }

    public void ShowLearningCard(string category)
    {
        if (_isAnimating || !_allCards.ContainsKey(category)) return;
        if (_isAnimating || !_allCards.ContainsKey(category) || !_currentIndex.ContainsKey(category))
            return;
        _currentCategory = category;
        var cards = _allCards[category];
        if (cards.Count == 0) return;

        // Получаем текущую карточку
        var texture = cards[_currentIndex[category]];
        StartCoroutine(ShowCardAnimation(texture));

        // Обновляем прогресс
        _currentIndex[category] = (_currentIndex[category] + 1) % cards.Count;
        SaveUserProgress();
        UpdateProgressSlider();
    }

    public void ShowRandomCard(string category)
    {
        if (_isAnimating || !_allCards.ContainsKey(category)) return;

        // Получаем просмотренные карточки из данных пользователя
        UserData userData = _dataManager.LoadUserData(_currentUserId);
        if (userData?.viewedImages == null || userData.viewedImages.Count == 0)
        {
            Debug.Log("Нет просмотренных карточек для повторения");
            return;
        }

        // Фильтруем только существующие карточки
        var availableCards = _allCards[category]
            .Where(card => userData.viewedImages.Contains(card.name))
            .ToList();

        if (availableCards.Count == 0)
        {
            Debug.Log("Нет доступных карточек для повторения в этой категории");
            return;
        }

        // Выбираем случайную карточку из просмотренных
        var texture = availableCards[Random.Range(0, availableCards.Count)];
        StartCoroutine(ShowCardAnimation(texture));
    }

    private IEnumerator ShowCardAnimation(Texture2D texture)
    {
        _isAnimating = true;
        ApplyTextureToCard(texture);

        // Уведомляем другие системы о новой карточке ↓
        FindObjectOfType<LastCardDisplay>()?.UpdateLastCard(texture);
        FindObjectOfType<AnswerChecker>()?.OnCardShown(texture);
        FindObjectOfType<AudioManager>()?.SetCurrentTexture(texture.name);

        card.transform.localScale = Vector3.zero;
        card.SetActive(true);

        yield return new WaitForSeconds(0.7f);
        LeanTween.scale(card, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);
        yield return new WaitForSeconds(2f);
        LeanTween.scale(card, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack)
            .setOnComplete(() => card.SetActive(false));
        yield return new WaitForSeconds(0.3f);
        _isAnimating = false;

        // Добавляем карточку в просмотренные
        AddToViewed(texture.name);
    }

    private void AddToViewed(string textureName)
    {
        if (_isGuestMode) return; // Не сохраняем

        UserData userData = _dataManager.LoadUserData(_currentUserId);
        if (userData.viewedImages == null)
        {
            userData.viewedImages = new List<string>();
        }

        if (!userData.viewedImages.Contains(textureName))
        {
            userData.viewedImages.Add(textureName);
            _dataManager.SaveUserData(userData);
        }
    }

    private void UpdateProgressSlider()
    {
        if (progressSlider == null || string.IsNullOrEmpty(_currentCategory)) return;

        var cards = _allCards[_currentCategory];
        if (cards.Count == 0) return;

        progressSlider.value = (float)_currentIndex[_currentCategory] / cards.Count;
    }

    private void ApplyTextureToCard(Texture2D texture)
    {
        if (card.TryGetComponent(out Renderer renderer))
        {
            renderer.material = renderer.material ?? new Material(Shader.Find("Standard"));
            renderer.material.mainTexture = texture;
        }
    }

    // Методы для UI кнопок
    public void ShowAnimal() => ShowLearningCard("Animals");
    public void ShowRAnimal() => ShowRandomCard("Animals");
    public void ShowVerb() => ShowLearningCard("Verbs");
    public void ShowRVerb() => ShowRandomCard("Verbs");
    public void ShowAdjective() => ShowLearningCard("Abjectives");
    public void ShowRAdjective() => ShowRandomCard("Abjectives");
    public void ShowColors() => ShowLearningCard("Colors");
    public void ShowRColors() => ShowRandomCard("Colors");
    public void ShowPeople() => ShowLearningCard("People");
    public void ShowRPeople() => ShowRandomCard("People");
    public void ShowObjects() => ShowLearningCard("Objects");
    public void ShowRObjects() => ShowRandomCard("Objects");

    public void ResetCategory(string category)
    {
        if (_currentIndex.ContainsKey(category))
        {
            _currentIndex[category] = 0;
            SaveUserProgress();
            UpdateProgressSlider();
        }
    }

    public void ResetAllProgress()
    {
        foreach (var category in _currentIndex.Keys)
        {
            _currentIndex[category] = 0;
        }
        SaveUserProgress();
        UpdateProgressSlider();
    }
    public string GetCurrentCardName()
    {
        if (card.TryGetComponent<Renderer>(out var renderer) && renderer.material.mainTexture != null)
        {
            return renderer.material.mainTexture.name;
        }
        return "";

    }
    public bool IsCardActive()
    {
        return card != null && card.activeInHierarchy;
    }
}