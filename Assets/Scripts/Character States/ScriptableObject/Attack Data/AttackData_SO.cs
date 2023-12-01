using UnityEngine;

namespace RPG.SO
{
    [CreateAssetMenu(fileName = "New Attack Data", menuName = "Character States/Attack Data")]
    public class AttackData_SO : ScriptableObject
    {
        // ��ͨ������Χ
        public float AttackRange;
        // ���ܹ�����Χ
        public float SkillRange;
        // cd��ȴ
        public float Cooldown;
        // ��С�˺�
        public float MinDamage;
        // ����˺�
        public float MaxDamage;
        // �����˻�
        public float CriticalMultiplier;
        // ������
        public float CriticalChance;
    }
}