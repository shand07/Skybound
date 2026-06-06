using Skybound.Core.Diagnostics;
using UnityEngine;

namespace Skybound.Data
{
    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "Skybound/Characters/Character Data")]
    public class CharacterData : ScriptableObject
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

        private void OnValidate()
        {
            ValidateInEditor();
        }

        public bool IsValid(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(characterName))
            {
                errorMessage = "Character name is empty.";
                return false;
            }

            if (strength < 1)
            {
                errorMessage = "Strength must be at least 1.";
                return false;
            }

            if (dexterity < 1)
            {
                errorMessage = "Dexterity must be at least 1.";
                return false;
            }

            if (constitution < 1)
            {
                errorMessage = "Constitution must be at least 1.";
                return false;
            }

            if (intelligence < 1)
            {
                errorMessage = "Intelligence must be at least 1.";
                return false;
            }

            if (baseHealth <= 0)
            {
                errorMessage = "Base health must be greater than 0.";
                return false;
            }

            if (healthPerConstitution < 0)
            {
                errorMessage = "Health per constitution cannot be negative.";
                return false;
            }

            if (baseArmorClass < 0)
            {
                errorMessage = "Base armor class cannot be negative.";
                return false;
            }

            if (baseDamageReduction < 0)
            {
                errorMessage = "Base damage reduction cannot be negative.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private void ValidateInEditor()
        {
            if (!IsValid(out string errorMessage))
                SkyboundDebug.Warning($"{name} CharacterData invalid: {errorMessage}", this);
        }
    }
}