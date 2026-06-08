using UnityEngine;

namespace Skybound.Data
{
    [CreateAssetMenu(fileName = "NewTalentTreeData", menuName = "Skybound/Talents/Talent Tree Data")]
    public class TalentTreeData : SkyboundDataAsset
    {
        [Header("Identity")]
        [SerializeField] private string treeName;

        [TextArea]
        [SerializeField] private string description;

        [Header("Talents")]
        [SerializeField] private TalentData[] talents;

        public string TreeName => treeName;
        public string Description => description;
        public TalentData[] Talents => talents;

        protected override bool ValidateData(out string errorMessage)
        {
            if (!ValidateRequiredString(treeName, nameof(treeName), out errorMessage))
                return false;

            if (talents == null || talents.Length == 0)
            {
                errorMessage = "Talent tree must contain at least one talent.";
                return false;
            }

            for (int i = 0; i < talents.Length; i++)
            {
                TalentData talent = talents[i];

                if (talent == null)
                {
                    errorMessage = $"Talent at index {i} is null.";
                    return false;
                }

                if (!talent.IsValid(out string talentError))
                {
                    errorMessage = $"Talent '{talent.name}' is invalid: {talentError}";
                    return false;
                }
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}