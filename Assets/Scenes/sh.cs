using UnityEngine;

public class AnimationSwitcher : MonoBehaviour
{
    public Animator animator; // Ссылка на компонент Animator
    private int animationCount = 0; // Счетчик повторений анимации
    public string animationTrigger = "SwitchAnimation"; // Имя триггера для смены анимации

    void Update()
    {
        // Проверяем, воспроизводится ли текущая анимация
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Дыхание"))
        {
            // Увеличиваем счетчик, если анимация воспроизводится
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
            {
                animationCount++;
                animator.Play("Дыхание", 0, 0); // Сбрасываем анимацию для повторного воспроизведения
            }

            // Проверяем, достигли ли мы 4 повторений
            if (animationCount >= 4)
            {
                animator.SetTrigger(animationTrigger); // Смена анимации
                animationCount = 0; // Сбрасываем счетчик
            }
        }
    }
}