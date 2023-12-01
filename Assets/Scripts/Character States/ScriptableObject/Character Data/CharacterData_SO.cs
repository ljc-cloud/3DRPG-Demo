using UnityEngine;

namespace RPG.SO
{
    [CreateAssetMenu(fileName = "New Character Data", menuName = "Character States/Character Data")]
    public class CharacterData_SO : ScriptableObject
    {
        [Header("Stats Info")]
        // 最大血量
        public float MaxHealth;
        // 当前血量
        public float CurrentHealth;
        // 基础防御值
        public float BaseDefense;
        // 当前防御值
        public float CurrentDefense;
    }

}