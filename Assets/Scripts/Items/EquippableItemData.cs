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
    }
}