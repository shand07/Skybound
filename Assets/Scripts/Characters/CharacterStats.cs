using UnityEngine;

namespace Skybound.Characters
{
    public enum AttributeType
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence
    }

    public class CharacterStats : MonoBehaviour
    {
        [Header("Attributes")]
        [SerializeField] private int strength = 10;
        [SerializeField] private int dexterity = 10;
        [SerializeField] private int constitution = 10;
        [SerializeField] private int intelligence = 10;

        [Header("Health")]
        [SerializeField] private int baseHealth = 50;
        [SerializeField] private int healthPerConstitution = 5;

        [Header("Combat")]
        [SerializeField] private int baseAccuracy = 0;
        [SerializeField] private int baseArmorClass = 10;
        [SerializeField] private int baseDamageReduction = 0;

        [Header("Runtime")]
        [SerializeField] private int maxHealth;
        [SerializeField] private int currentHealth;
        [SerializeField] private int accuracy;
        [SerializeField] private int armorClass;
        [SerializeField] private int damageReduction;

        public int Strength => strength;
        public int Dexterity => dexterity;
        public int Constitution => constitution;
        public int Intelligence => intelligence;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public int Accuracy => accuracy;
        public int ArmorClass => armorClass;
        public int DamageReduction => damageReduction;

        public bool IsDead => currentHealth <= 0;

        private void Awake()
        {
            RecalculateStats();
            currentHealth = maxHealth;
        }

        public void RecalculateStats()
        {
            maxHealth = baseHealth + constitution * healthPerConstitution;

            accuracy = baseAccuracy + GetAttributeModifier(dexterity);
            armorClass = baseArmorClass + GetAttributeModifier(dexterity);
            damageReduction = baseDamageReduction;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
        }

        public int GetAttribute(AttributeType attributeType)
        {
            return attributeType switch
            {
                AttributeType.Strength => strength,
                AttributeType.Dexterity => dexterity,
                AttributeType.Constitution => constitution,
                AttributeType.Intelligence => intelligence,
                _ => 0
            };
        }

        public int GetAttributeModifier(AttributeType attributeType)
        {
            return GetAttributeModifier(GetAttribute(attributeType));
        }

        public int GetAttributeModifier(int attributeValue)
        {
            return Mathf.FloorToInt((attributeValue - 10) / 2f);
        }

        public int GetMeleeScalingStat()
        {
            return Mathf.Max(strength, dexterity);
        }

        public int GetMeleeDamageModifier()
        {
            return GetAttributeModifier(GetMeleeScalingStat());
        }

        public int RollD20()
        {
            return Random.Range(1, 21);
        }

        public bool RollAttackAgainst(CharacterStats defender, out int roll, out bool isCritical, out bool isNaturalMiss)
        {
            roll = RollD20();

            isCritical = roll == 20;
            isNaturalMiss = roll == 1;

            if (isNaturalMiss)
                return false;

            if (isCritical)
                return true;

            int attackTotal = roll + accuracy;
            return attackTotal >= defender.ArmorClass;
        }

        public int ReduceIncomingDamage(int rawDamage)
        {
            return Mathf.Max(0, rawDamage - damageReduction);
        }

        public void TakeDamage(int rawDamage)
        {
            if (IsDead)
                return;

            int finalDamage = ReduceIncomingDamage(rawDamage);

            currentHealth -= finalDamage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            Debug.Log($"{name} took {finalDamage} damage. HP: {currentHealth}/{maxHealth}");

            if (IsDead)
                Die();
        }

        public void Heal(int amount)
        {
            if (IsDead)
                return;

            currentHealth += Mathf.Max(0, amount);
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        private void Die()
        {
            Debug.Log($"{name} died.");
            gameObject.SetActive(false);
        }
    }
}