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
    }
}