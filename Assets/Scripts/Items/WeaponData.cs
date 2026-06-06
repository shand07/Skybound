using Skybound.Core.Diagnostics;
using UnityEngine;

namespace Skybound.Items
{
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "Skybound/Items/Weapon Data")]
    public class WeaponData : EquippableItemData
    {
        [Header("Weapon Stats")]
        [SerializeField] private int baseDamage = 6;
        [SerializeField] private int accuracyBonus = 0;
        [SerializeField] private float attacksPerRoundBonus = 0f;

        [Header("Range")]
        [SerializeField] private float attackRange = 2f;

        public int BaseDamage => baseDamage;
        public int AccuracyBonus => accuracyBonus;
        public float AttacksPerRoundBonus => attacksPerRoundBonus;
        public float AttackRange => attackRange;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!IsValid(out string errorMessage))
                SkyboundDebug.Warning($"{name} WeaponData invalid: {errorMessage}", this);
        }

        public override bool IsValid(out string errorMessage)
        {
            if (!base.IsValid(out errorMessage))
                return false;

            if (baseDamage < 0)
            {
                errorMessage = "Base damage cannot be negative.";
                return false;
            }

            if (attacksPerRoundBonus < 0f)
            {
                errorMessage = "Attacks per round bonus cannot be negative.";
                return false;
            }

            if (attackRange <= 0f)
            {
                errorMessage = "Attack range must be greater than 0.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}