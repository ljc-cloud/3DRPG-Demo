using UnityEngine;

namespace RPG.SO
{
    [CreateAssetMenu(fileName = "New Attack Data", menuName = "Character States/Attack Data")]
    public class AttackData_SO : ScriptableObject
    {
        // ÆÕÍ¨¹¥»÷·¶Î§
        public float AttackRange;
        // ¼¼ÄÜ¹¥»÷·¶Î§
        public float SkillRange;
        // cdÀäÈ´
        public float Cooldown;
        // ×îĞ¡ÉËº¦
        public float MinDamage;
        // ×î´óÉËº¦
        public float MaxDamage;
        // ±©»÷³Ë»ı
        public float CriticalMultiplier;
        // ±©»÷ÂÊ
        public float CriticalChance;
    }
}