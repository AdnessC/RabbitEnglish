using UnityEngine;

public class AnimationSwitcher : MonoBehaviour
{
    public Animator animator; // ������ �� ��������� Animator
    private int animationCount = 0; // ������� ���������� ��������
    public string animationTrigger = "SwitchAnimation"; // ��� �������� ��� ����� ��������

    void Update()
    {
        // ���������, ��������������� �� ������� ��������
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("�������"))
        {
            // ����������� �������, ���� �������� ���������������
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
            {
                animationCount++;
                animator.Play("�������", 0, 0); // ���������� �������� ��� ���������� ���������������
            }

            // ���������, �������� �� �� 4 ����������
            if (animationCount >= 4)
            {
                animator.SetTrigger(animationTrigger); // ����� ��������
                animationCount = 0; // ���������� �������
            }
        }
    }
}