using System;

namespace Skybound.Data.Json
{
    [Serializable]
    public class CharacterDefinition : IDataDefinition
    {
        public string id;
        public string displayName;
        public string portraitId;
        public string dialogueProfileId;

        public CharacterAttributeDefinition attributes;
        public CharacterHealthDefinition health;
        public CharacterCombatDefinition combat;
        public CharacterStartingEquipmentDefinition startingEquipment;

        public string Id => id;
        public string DisplayName => displayName;
        public string PortraitId => portraitId;
        public string DialogueProfileId => dialogueProfileId;

        public CharacterAttributeDefinition Attributes => attributes;
        public CharacterHealthDefinition Health => health;
        public CharacterCombatDefinition Combat => combat;

        public CharacterStartingEquipmentDefinition StartingEquipment =>
            startingEquipment;

        public bool IsValid(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                errorMessage = "Character id is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                errorMessage =
                    $"Character '{id}' must have a display name.";

                return false;
            }

            if (attributes == null)
            {
                errorMessage =
                    $"Character '{id}' is missing its attributes section.";

                return false;
            }

            if (!attributes.IsValid(out string attributeError))
            {
                errorMessage =
                    $"Character '{id}' has invalid attributes: {attributeError}";

                return false;
            }

            if (health == null)
            {
                errorMessage =
                    $"Character '{id}' is missing its health section.";

                return false;
            }

            if (!health.IsValid(out string healthError))
            {
                errorMessage =
                    $"Character '{id}' has invalid health data: {healthError}";

                return false;
            }

            if (combat == null)
            {
                errorMessage =
                    $"Character '{id}' is missing its combat section.";

                return false;
            }

            if (!combat.IsValid(out string combatError))
            {
                errorMessage =
                    $"Character '{id}' has invalid combat data: {combatError}";

                return false;
            }

            if (startingEquipment == null)
            {
                errorMessage =
                    $"Character '{id}' is missing its startingEquipment section.";

                return false;
            }

            if (!startingEquipment.IsValid(out string equipmentError))
            {
                errorMessage =
                    $"Character '{id}' has invalid starting equipment: " +
                    equipmentError;

                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }

    [Serializable]
    public class CharacterAttributeDefinition
    {
        public int strength = 10;
        public int dexterity = 10;
        public int constitution = 10;
        public int intelligence = 10;

        public int Strength => strength;
        public int Dexterity => dexterity;
        public int Constitution => constitution;
        public int Intelligence => intelligence;

        public bool IsValid(out string errorMessage)
        {
            if (strength < 1)
            {
                errorMessage =
                    $"Strength must be at least 1. Current value: {strength}.";

                return false;
            }

            if (dexterity < 1)
            {
                errorMessage =
                    $"Dexterity must be at least 1. Current value: {dexterity}.";

                return false;
            }

            if (constitution < 1)
            {
                errorMessage =
                    $"Constitution must be at least 1. Current value: " +
                    $"{constitution}.";

                return false;
            }

            if (intelligence < 1)
            {
                errorMessage =
                    $"Intelligence must be at least 1. Current value: " +
                    $"{intelligence}.";

                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }

    [Serializable]
    public class CharacterHealthDefinition
    {
        public int baseHealth = 50;
        public int healthPerConstitution = 5;

        public int BaseHealth => baseHealth;

        public int HealthPerConstitution =>
            healthPerConstitution;

        public bool IsValid(out string errorMessage)
        {
            if (baseHealth <= 0)
            {
                errorMessage =
                    $"Base health must be greater than 0. Current value: " +
                    $"{baseHealth}.";

                return false;
            }

            if (healthPerConstitution < 0)
            {
                errorMessage =
                    $"Health per Constitution cannot be negative. " +
                    $"Current value: {healthPerConstitution}.";

                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }

    [Serializable]
    public class CharacterCombatDefinition
    {
        public int baseAccuracy;
        public int baseArmorClass = 10;
        public int baseDamageReduction;

        public int BaseAccuracy => baseAccuracy;
        public int BaseArmorClass => baseArmorClass;
        public int BaseDamageReduction => baseDamageReduction;

        public bool IsValid(out string errorMessage)
        {
            if (baseArmorClass < 0)
            {
                errorMessage =
                    $"Base Armor Class cannot be negative. Current value: " +
                    $"{baseArmorClass}.";

                return false;
            }

            if (baseDamageReduction < 0)
            {
                errorMessage =
                    $"Base damage reduction cannot be negative. " +
                    $"Current value: {baseDamageReduction}.";

                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }

    [Serializable]
    public class CharacterStartingEquipmentDefinition
    {
        public string mainHandItemId;
        public string offHandItemId;

        public string MainHandItemId => mainHandItemId;
        public string OffHandItemId => offHandItemId;

        public bool IsValid(out string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(mainHandItemId) &&
                !IsValidId(mainHandItemId))
            {
                errorMessage =
                    $"Main-hand item id '{mainHandItemId}' contains " +
                    "unsupported characters.";

                return false;
            }

            if (!string.IsNullOrWhiteSpace(offHandItemId) &&
                !IsValidId(offHandItemId))
            {
                errorMessage =
                    $"Off-hand item id '{offHandItemId}' contains " +
                    "unsupported characters.";

                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private bool IsValidId(string value)
        {
            foreach (char character in value)
            {
                bool isValidCharacter =
                    char.IsLetterOrDigit(character) ||
                    character == '_' ||
                    character == '-';

                if (!isValidCharacter)
                    return false;
            }

            return true;
        }
    }
}