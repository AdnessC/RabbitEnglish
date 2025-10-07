using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioCategory
    {
        public string name; 
        public string audioFolder; 
        public Button playButton; // Кнопка для воспроизведения
    }
    [System.Serializable]
    public class ExtraButtonBinding
    {
        public string categoryName; // Имя категории для этой кнопки
        public Button button;       // Дополнительная кнопка
        
    }

    [Header("Настройки")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float playDelay = 0.05f; // Задержка перед воспроизведением
    [SerializeField] private List<AudioCategory> categories = new List<AudioCategory>();

    [Header("Привязка кнопок к категориям")]
    [SerializeField] private ExtraButtonBinding[] buttonBindings;

    [Header("Дополнительные кнопки")]
    [SerializeField] private Button[] extraButtons; // Новые кнопки
    [SerializeField] private string extraSoundName = "Переведите это";

    private AudioClip _extraClip; // Звук для дополнительных кнопок
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    private string _currentTextureName;
    private Coroutine _playRoutine;

    void Start()
    {
        PreloadAllAudio();
        SetupButtons();
        SetupExtraButtons();
    }
    private void SetupExtraButtons()
    {
        _extraClip = Resources.Load<AudioClip>($"Audio/{extraSoundName}");
        if (_extraClip == null)
            Debug.LogError($"Звук '{extraSoundName}' не найден!");

        // Проверяем, что массив привязок не пуст
        if (buttonBindings == null || buttonBindings.Length == 0)
        {
            Debug.LogError("Не заданы привязки кнопок к категориям!");
            return;
        }

        foreach (var binding in buttonBindings)
        {
            if (binding.button == null)
            {
                Debug.LogWarning("Одна из кнопок не задана!");
                continue;
            }

            // Проверяем, что категория существует
            bool categoryExists = categories.Exists(c => c.name == binding.categoryName);
            if (!categoryExists)
            {
                Debug.LogWarning($"Категория '{binding.categoryName}' не найдена!");
                continue;
            }

            binding.button.onClick.AddListener(() =>
            {
                CardController cardController = FindObjectOfType<CardController>();
                if (cardController == null) return;

                cardController.ShowRandomCard(binding.categoryName);

                if (cardController.card.activeInHierarchy)
                {
                    StartCoroutine(PlayExtraSoundWithDelay());
                }
                else
                {
                    Debug.LogWarning($"В категории '{binding.categoryName}' нет карточек!");
                }
            });
        }
    }

    private void SetupButtons()
    {
        foreach (var category in categories)
        {
            if (category.playButton != null)
            {
                category.playButton.onClick.AddListener(() =>
                {
                    if (_playRoutine != null)
                        StopCoroutine(_playRoutine);

                    _playRoutine = StartCoroutine(PlayWithDelay());
                });
            }
        }
    }

    private IEnumerator PlayExtraSoundWithDelay()
    {
        yield return new WaitForSeconds(playDelay);

        if (_extraClip != null)
            audioSource.PlayOneShot(_extraClip);
    }

    private IEnumerator PlayWithDelay()
    {
        yield return new WaitForSeconds(playDelay); // Ждём 0.1 секунды

        if (_audioClips.TryGetValue(_currentTextureName.ToLower(), out AudioClip clip))
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Аудио для '{_currentTextureName}' не найдено!");
        }
    }

    public void SetCurrentTexture(string textureName)
    {
        _currentTextureName = System.IO.Path.GetFileNameWithoutExtension(textureName);
    }

    private void PreloadAllAudio()
    {
        foreach (var category in categories)
        {
            AudioClip[] clips = Resources.LoadAll<AudioClip>(category.audioFolder);
            foreach (var clip in clips)
            {
                string key = System.IO.Path.GetFileNameWithoutExtension(clip.name).ToLower();
                if (!_audioClips.ContainsKey(key))
                {
                    _audioClips.Add(key, clip);
                }
            }
        }
        Debug.Log($"Аудио загружено: {_audioClips.Count} файлов");
    }
}