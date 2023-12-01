using UnityEngine;

namespace RPG.SO
{
    [CreateAssetMenu(fileName = "New Character Data", menuName = "Character States/Character Data")]
    public class CharacterData_SO : ScriptableObject
    {
        [Header("Stats Info")]
        // ���Ѫ��
        public float MaxHealth;
        // ��ǰѪ��
        public float CurrentHealth;
        // ��������ֵ
        public float BaseDefense;
        // ��ǰ����ֵ
        public float CurrentDefense;
    }

}