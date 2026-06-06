using Skybound.Core.Diagnostics;
using Skybound.Items;
using UnityEngine;

namespace Skybound.Characters
{
    public class CharacterEquipment : MonoBehaviour
    {
        [Header("Hands")]
        [SerializeField] private EquippableItemData mainHand;
        [SerializeField] private EquippableItemData offHand;

        public EquippableItemData MainHand => mainHand;
        public EquippableItemData OffHand => offHand;

        public WeaponData MainHandWeapon => mainHand as WeaponData;
        public WeaponData OffHandWeapon => offHand as WeaponData;

        private void Awake()
        {
            ValidateEquipment();
        }

        public bool TryEquip(EquippableItemData item, EquipmentSlot slot)
        {
            if (item == null)
            {
                SkyboundDebug.Warning($"{name} tried to equip a null item.", this);
                return false;
            }

            if (!item.IsValid(out string errorMessage))
            {
                SkyboundDebug.Error(
                    $"{name} tried to equip invalid item '{item.name}': {errorMessage}",
                    this
                );

                return false;
            }

            if (!CanEquip(item, slot))
            {
                SkyboundDebug.Warning(
                    $"{name} cannot equip '{item.ItemName}' in slot {slot}. Handedness: {item.Handedness}.",
                    this
                );

                return false;
            }

            if (item.IsTwoHanded)
            {
                mainHand = item;
                offHand = null;

                SkyboundDebug.Log($"{name} equipped two-handed item '{item.ItemName}'. Off-hand cleared.", this);
                return true;
            }

            switch (slot)
            {
                case EquipmentSlot.MainHand:
                    mainHand = item;
                    SkyboundDebug.Log($"{name} equipped '{item.ItemName}' in main hand.", this);
                    break;

                case EquipmentSlot.OffHand:
                    offHand = item;
                    SkyboundDebug.Log($"{name} equipped '{item.ItemName}' in off hand.", this);
                    break;

                default:
                    SkyboundDebug.Warning($"{name} tried to equip '{item.ItemName}' into unknown slot {slot}.", this);
                    return false;
            }

            ValidateEquipment();
            return true;
        }

        public void Unequip(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.MainHand:
                    if (mainHand != null)
                        SkyboundDebug.Log($"{name} unequipped '{mainHand.ItemName}' from main hand.", this);

                    mainHand = null;
                    break;

                case EquipmentSlot.OffHand:
                    if (offHand != null)
                        SkyboundDebug.Log($"{name} unequipped '{offHand.ItemName}' from off hand.", this);

                    offHand = null;
                    break;

                default:
                    SkyboundDebug.Warning($"{name} tried to unequip unknown slot {slot}.", this);
                    break;
            }
        }

        public bool CanEquip(EquippableItemData item, EquipmentSlot slot)
        {
            if (item == null)
                return false;

            if (!item.IsValid(out _))
                return false;

            if (item.Handedness == HandednessType.TwoHanded)
                return slot == EquipmentSlot.MainHand;

            if (item.Handedness == HandednessType.OffHandOnly)
                return slot == EquipmentSlot.OffHand;

            if (slot == EquipmentSlot.OffHand && mainHand != null && mainHand.IsTwoHanded)
                return false;

            return true;
        }

        private void ValidateEquipment()
        {
            if (mainHand != null && !mainHand.IsValid(out string mainHandError))
            {
                SkyboundDebug.Error(
                    $"{name} has invalid main-hand item '{mainHand.name}': {mainHandError}. Clearing main hand.",
                    this
                );

                mainHand = null;
            }

            if (offHand != null && !offHand.IsValid(out string offHandError))
            {
                SkyboundDebug.Error(
                    $"{name} has invalid off-hand item '{offHand.name}': {offHandError}. Clearing off hand.",
                    this
                );

                offHand = null;
            }

            if (mainHand != null && mainHand.IsTwoHanded && offHand != null)
            {
                SkyboundDebug.Warning(
                    $"{name} had a two-handed main-hand item and an off-hand item. Clearing off hand.",
                    this
                );

                offHand = null;
            }

            if (offHand != null && offHand.Handedness == HandednessType.TwoHanded)
            {
                SkyboundDebug.Warning(
                    $"{name} had a two-handed item in off hand. Clearing off hand.",
                    this
                );

                offHand = null;
            }

            if (mainHand != null && mainHand.Handedness == HandednessType.OffHandOnly)
            {
                SkyboundDebug.Warning(
                    $"{name} had an off-hand-only item in main hand. Clearing main hand.",
                    this
                );

                mainHand = null;
            }
        }

        public int GetMainHandDamage()
        {
            return MainHandWeapon != null ? MainHandWeapon.BaseDamage : 1;
        }

        public int GetMainHandAccuracyBonus()
        {
            return MainHandWeapon != null ? MainHandWeapon.AccuracyBonus : 0;
        }

        public float GetMainHandAttackRange()
        {
            return MainHandWeapon != null ? MainHandWeapon.AttackRange : 2f;
        }

        public float GetMainHandAttacksPerRoundBonus()
        {
            return MainHandWeapon != null ? MainHandWeapon.AttacksPerRoundBonus : 0f;
        }

        public float GetOffHandPowerAttacksPerRoundBonus()
        {
            return OffHandWeapon != null ? OffHandWeapon.AttacksPerRoundBonus : 0f;
        }

        public int GetOffHandDamage()
        {
            return OffHandWeapon != null ? OffHandWeapon.BaseDamage : 1;
        }

        public int GetOffHandAccuracyBonus()
        {
            return OffHandWeapon != null ? OffHandWeapon.AccuracyBonus : 0;
        }

        public bool HasOffHandAttack()
        {
            return MainHandWeapon != null && OffHandWeapon != null;
        }

        public float GetTotalDisplayedAttacksPerRoundBonus()
        {
            float bonus = 0f;

            bonus += GetMainHandAttacksPerRoundBonus();
            bonus += GetOffHandPowerAttacksPerRoundBonus();

            if (HasOffHandAttack())
                bonus += 1f;

            return bonus;
        }
    }
}