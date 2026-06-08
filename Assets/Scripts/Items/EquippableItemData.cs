using Skybound.Data;
using UnityEngine;

namespace Skybound.Items
{
    public abstract class EquippableItemData : SkyboundDataAsset
    {
        [Header("Item Identity")]
        [SerializeField] private string itemName;

        [Header("Hand Rules")]
        [SerializeField] private HandednessType handedness = HandednessType.OneHanded;

        public string ItemName => itemName;
        public HandednessType Handedness => handedness;

        public bool IsTwoHanded => handedness == HandednessType.TwoHanded;

        protected override bool ValidateData(out string errorMessage)
        {
            if (!ValidateRequiredString(itemName, nameof(itemName), out errorMessage))
                return false;

            errorMessage = string.Empty;
            return true;
        }
    }
}