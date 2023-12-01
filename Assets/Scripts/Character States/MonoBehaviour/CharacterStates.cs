using RPG.SO;
using UnityEngine;

namespace RPG.State
{
    public class CharacterStates : MonoBehaviour
    {
        public CharacterData_SO TempCharacterData;
        private CharacterData_SO CharacterData;
        public AttackData_SO AttackData;

        [HideInInspector]
        public bool IsCritical;

        private void Awake()
        {
            CharacterData = TempCharacterData;
        }

        public float MaxHealth
        {
            get
            {
                if (CharacterData != null)
                    return CharacterData.MaxHealth;
                return 0;
            }
            set
            {
                CharacterData.MaxHealth = value;
            }
        }

        public float CurrentHealth
        {
            get
            {
                if (CharacterData != null)
                    return CharacterData.CurrentHealth;
                return 0;
            }
            set
            {
                CharacterData.CurrentHealth = value;
            }
        }
        public float BaseDefense
        {
            get
            {
                if (CharacterData != null)
                    return CharacterData.BaseDefense;
                return 0;
            }
            set
            {
                CharacterData.BaseDefense = value;
            }
        }
        public float CurrentDefense
        {
            get
            {
                if (CharacterData != null)
                    return CharacterData.CurrentDefense;
                return 0;
            }
            set
            {
                CharacterData.CurrentDefense = value;
            }
        }
        public float AttackRange
        {
            get
            {
                if (AttackData != null)
                    return AttackData.AttackRange;
                return 0;
            }
            set
            {
                AttackData.AttackRange = value;
            }
        }
        public float SkillRange
        {
            get
            {
                if (AttackData != null)
                    return AttackData.SkillRange;
                return 0;
            }
            set
            {
                AttackData.SkillRange = value;
            }
        }
        public float Cooldown
        {
            get
            {
                if (AttackData != null)
                    return AttackData.Cooldown;
                return 0;
            }
            set
            {
                AttackData.Cooldown = value;
            }
        }
        public float MinDamage
        {
            get
            {
                if (AttackData != null)
                    return AttackData.MinDamage;
                return 0;
            }
            set
            {
                AttackData.MinDamage = value;
            }
        }
        public float MaxDamage
        {
            get
            {
                if (AttackData != null)
                    return AttackData.MaxDamage;
                return 0;
            }
            set
            {
                AttackData.MaxDamage = value;
            }
        }
        public float CriticalMultiplier
        {
            get
            {
                if (AttackData != null)
                    return AttackData.CriticalMultiplier;
                return 0;
            }
            set
            {
                AttackData.CriticalMultiplier = value;
            }
        }
        public float CriticalChance
        {
            get
            {
                if (AttackData != null)
                    return AttackData.CriticalChance;
                return 0;
            }
            set
            {
                AttackData.CriticalChance = value;
            }
        }

        public void TakeDamage(CharacterStates attacker, CharacterStates defender)
        {

            //float damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefense, 0);
            float damage = attacker.CurrentDamage() - defender.CurrentDefense;
            float finalDamage = damage > 0 ? damage : Mathf.Abs(damage) / defender.CurrentDefense;
            defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - finalDamage, 0);

            if (defender.CurrentHealth <= 0)
            {
                defender.GetComponent<Animator>().SetBool("die", true);
            }
            // todo Update UI
            // todo Update 经验
        }

        public void TakeDamage(float damage, CharacterStates defender)
        {
            float finalDamage = (damage - defender.CurrentDefense) > 0 ? damage - defender.CurrentDefense : Mathf.Abs(damage) / defender.CurrentDefense;
            defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - finalDamage, 0);
            
            if (defender.CurrentHealth <= 0)
            {
                defender.GetComponent<Animator>().SetBool("die", true);
            }
            // todo Update UI
            // todo Update 经验
        }

        private float CurrentDamage()
        {
            float coreDamage = Random.Range(MinDamage, MaxDamage);
            float damage = IsCritical ? coreDamage * CriticalMultiplier : coreDamage;
            return damage;
        }
    }
}