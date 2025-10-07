using UnityEngine;
using System.Collections.Generic;

public class CardMemoryManager : MonoBehaviour
{
    private static CardMemoryManager _instance;
    public static CardMemoryManager Instance => _instance;

    // Словарь для хранения просмотренных карточек по категориям
    private Dictionary<string, HashSet<string>> _viewedCards = new Dictionary<string, HashSet<string>>();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Инициализация категорий
        InitCategory("Animals");
        InitCategory("Verbs");
        InitCategory("Adjectives");
        InitCategory("Colors");
        InitCategory("People");
        InitCategory("Objects");
    }

    private void InitCategory(string category)
    {
        if (!_viewedCards.ContainsKey(category))
        {
            _viewedCards.Add(category, new HashSet<string>());
        }
    }

    // Добавляем карточку в просмотренные
    public void AddViewedCard(string category, string cardName)
    {
        if (_viewedCards.TryGetValue(category, out var categorySet))
        {
            categorySet.Add(cardName);
            Debug.Log($"Добавлена карточка '{cardName}' в категорию '{category}'");
        }
    }

    // Получаем все просмотренные карточки категории
    public HashSet<string> GetViewedCards(string category)
    {
        if (_viewedCards.TryGetValue(category, out var categorySet))
        {
            return new HashSet<string>(categorySet);
        }
        return new HashSet<string>();
    }

    // Очищаем историю по категории
    public void ClearCategory(string category)
    {
        if (_viewedCards.ContainsKey(category))
        {
            _viewedCards[category].Clear();
        }
    }
