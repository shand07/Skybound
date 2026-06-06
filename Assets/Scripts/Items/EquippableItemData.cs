using Skybound.Core.Diagnostics;
using UnityEngine;

namespace Skybound.Items
{
    public abstract class EquippableItemData : ScriptableObject
    {
        [Header("Item Identity")]
        [SerializeField] private string itemName;

        [Header("Hand Rules")]
        [SerializeField] private HandednessType handedness = HandednessType.OneHanded;

        public string ItemName => itemName;
        public HandednessType Handedness => handedness;

        public bool IsTwoHanded => handedness == HandednessType.TwoHanded;

        protected virtual void OnValidate()
        {
            ValidateInEditor();
        }

        public virtual bool IsValid(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                errorMessage = "Item name is empty.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        protected void ValidateInEditor()
        {
            if (!IsValid(out string errorMessage))
                SkyboundDebug.Warning($"{name} EquippableItemData invalid: {errorMessage}", this);
        }
    }
}