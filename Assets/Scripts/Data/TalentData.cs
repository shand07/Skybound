using UnityEngine;

namespace Skybound.Data
{
    public enum TalentEffectType
    {
        PassiveStatBonus,
        UnlockAbility,
        ModifyAbility
    }

    [CreateAssetMenu(fileName = "NewTalentData", menuName = "Skybound/Talents/Talent Data")]
    public class TalentData : SkyboundDataAsset
    {
        [Header("Identity")]
        [SerializeField] private string talentName;

        [TextArea]
        [SerializeField] private string description;

        [Header("Progression")]
        [SerializeField] private int maxRank = 1;
        [SerializeField] private int requiredCharacterLevel = 1;
        [SerializeField] private TalentData[] prerequisiteTalents;

        [Header("Effect")]
        [SerializeField] private TalentEffectType effectType = TalentEffectType.PassiveStatBonus;
        [SerializeField] private AbilityData unlockedAbility;

        public string TalentName => talentName;
        public string Description => description;

        public int MaxRank => maxRank;
        public int RequiredCharacterLevel => requiredCharacterLevel;
        public TalentData[] PrerequisiteTalents => prerequisiteTalents;

        public TalentEffectType EffectType => effectType;
        public AbilityData UnlockedAbility => unlockedAbility;

        protected override bool ValidateData(out string errorMessage)
        {
            if (!ValidateRequiredString(talentName, nameof(talentName), out errorMessage))
                return false;

            if (!ValidateGreaterThanZero(maxRank, nameof(maxRank), out errorMessage))
                return false;

            if (!ValidateGreaterThanZero(requiredCharacterLevel, nameof(requiredCharacterLevel), out errorMessage))
                return false;

            if (effectType == TalentEffectType.UnlockAbility && unlockedAbility == null)
            {
                errorMessage = "UnlockAbility talent requires an AbilityData reference.";
                return false;
            }

            if (unlockedAbility != null && !unlockedAbility.IsValid(out string abilityError))
            {
                errorMessage = $"Unlocked ability '{unlockedAbility.name}' is invalid: {abilityError}";
                return false;
            }

            if (prerequisiteTalents != null)
            {
                for (int i = 0; i < prerequisiteTalents.Length; i++)
                {
                    if (prerequisiteTalents[i] == null)
                    {
                        errorMessage = $"Prerequisite talent at index {i} is null.";
                        return false;
                    }
                }
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}