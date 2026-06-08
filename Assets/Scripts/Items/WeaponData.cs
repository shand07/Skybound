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

        protected override bool ValidateData(out string errorMessage)
        {
            if (!base.ValidateData(out errorMessage))
                return false;

            if (!ValidateMinimumInt(baseDamage, 0, nameof(baseDamage), out errorMessage))
                return false;

            if (!ValidateMinimumFloat(attacksPerRoundBonus, 0f, nameof(attacksPerRoundBonus), out errorMessage))
                return false;

            if (!ValidateGreaterThanZero(attackRange, nameof(attackRange), out errorMessage))
                return false;

            errorMessage = string.Empty;
            return true;
        }
    }
}