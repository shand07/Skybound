using UnityEngine;

namespace Skybound.Data
{
    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "Skybound/Characters/Character Data")]
    public class CharacterData : SkyboundDataAsset
    {
        [Header("Identity")]
        [SerializeField] private string characterName;

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

        public string CharacterName => characterName;

        public int Strength => strength;
        public int Dexterity => dexterity;
        public int Constitution => constitution;
        public int Intelligence => intelligence;

        public int BaseHealth => baseHealth;
        public int HealthPerConstitution => healthPerConstitution;

        public int BaseAccuracy => baseAccuracy;
        public int BaseArmorClass => baseArmorClass;
        public int BaseDamageReduction => baseDamageReduction;

        protected override bool ValidateData(out string errorMessage)
        {
            if (!ValidateRequiredString(characterName, nameof(characterName), out errorMessage))
                return false;

            if (!ValidateMinimumInt(strength, 1, nameof(strength), out errorMessage))
                return false;

            if (!ValidateMinimumInt(dexterity, 1, nameof(dexterity), out errorMessage))
                return false;

            if (!ValidateMinimumInt(constitution, 1, nameof(constitution), out errorMessage))
                return false;

            if (!ValidateMinimumInt(intelligence, 1, nameof(intelligence), out errorMessage))
                return false;

            if (!ValidateGreaterThanZero(baseHealth, nameof(baseHealth), out errorMessage))
                return false;

            if (!ValidateMinimumInt(healthPerConstitution, 0, nameof(healthPerConstitution), out errorMessage))
                return false;

            if (!ValidateMinimumInt(baseArmorClass, 0, nameof(baseArmorClass), out errorMessage))
                return false;

            if (!ValidateMinimumInt(baseDamageReduction, 0, nameof(baseDamageReduction), out errorMessage))
                return false;

            errorMessage = string.Empty;
            return true;
        }
    }
}