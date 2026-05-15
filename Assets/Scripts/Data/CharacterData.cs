using UnityEngine;

namespace Skybound.Data
{
    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "Skybound/Characters/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string characterName;

        [Header("Attributes")]
        [SerializeField] private int strength = 10;
        [SerializeField] private int dexterity = 10;
        [SerializeField] private int constitution = 10;
        [SerializeField] private int intelligence = 10;

        [Header("Health")]
        [SerializeField] private int baseHealth = 50;
        [SerializeField] private int healthPerConstitution = 5;

        [Header("Combat")]
        [SerializeField] private int baseAccuracy = 0;
        [SerializeField] private int baseArmorClass = 10;
        [SerializeField] private int baseDamageReduction = 0;

        public string CharacterName => characterName;

        public int Strength => strength;
        public int Dexterity => dexterity;
        public int Constitution => constitution;
        public int Intelligence => intelligence;

        public int BaseHealth => baseHealth;
        public int HealthPerConstitution => healthPerConstitution;

        public int BaseAccuracy => baseAccuracy;
        public int BaseArmorClass => baseArmorClass;
        public int BaseDamageReduction => baseDamageReduction;
    }
}