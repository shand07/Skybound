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
                return false;

            if (!CanEquip(item, slot))
                return false;

            if (item.IsTwoHanded)
            {
                mainHand = item;
                offHand = null;
                return true;
            }

            switch (slot)
            {
                case EquipmentSlot.MainHand:
                    mainHand = item;
                    break;

                case EquipmentSlot.OffHand:
                    offHand = item;
                    break;
            }

            return true;
        }

        public void Unequip(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.MainHand:
                    mainHand = null;
                    break;

                case EquipmentSlot.OffHand:
                    offHand = null;
                    break;
            }
        }

        public bool CanEquip(EquippableItemData item, EquipmentSlot slot)
        {
            if (item == null)
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
            if (mainHand != null && mainHand.IsTwoHanded)
            {
                offHand = null;
            }

            if (offHand != null && offHand.Handedness == HandednessType.TwoHanded)
            {
                offHand = null;
            }

            if (mainHand != null && mainHand.Handedness == HandednessType.OffHandOnly)
            {
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