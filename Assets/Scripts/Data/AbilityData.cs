using UnityEngine;

namespace Skybound.Data
{
    public enum AbilityTargetType
    {
        Self,
        Ally,
        Enemy,
        Ground
    }

    public enum AbilityResourceType
    {
        None,
        Mana,
        Health
    }

    public enum AbilityAreaShape
    {
        SingleTarget,
        Circle,
        Cone,
        Line,
        Rectangle
    }

    [CreateAssetMenu(fileName = "NewAbilityData", menuName = "Skybound/Abilities/Ability Data")]
    public class AbilityData : SkyboundDataAsset
    {
        [Header("Identity")]
        [SerializeField] private string abilityName;

        [TextArea]
        [SerializeField] private string description;

        [Header("Targeting")]
        [SerializeField] private AbilityTargetType targetType = AbilityTargetType.Enemy;
        [SerializeField] private float range = 8f;

        [Header("Area")]
        [SerializeField] private AbilityAreaShape areaShape = AbilityAreaShape.SingleTarget;
        [SerializeField] private float areaRadius = 0f;
        [SerializeField] private float coneAngle = 60f;
        [SerializeField] private float lineWidth = 1f;
        [SerializeField] private float rectangleWidth = 3f;
        [SerializeField] private float rectangleLength = 5f;

        [Header("Resource Cost")]
        [SerializeField] private AbilityResourceType resourceType = AbilityResourceType.Mana;
        [SerializeField] private int resourceCost = 0;

        [Header("Timing")]
        [SerializeField] private float castTime = 0f;
        [SerializeField] private float cooldown = 0f;

        public string AbilityName => abilityName;
        public string Description => description;

        public AbilityTargetType TargetType => targetType;
        public float Range => range;

        public AbilityAreaShape AreaShape => areaShape;
        public float AreaRadius => areaRadius;
        public float ConeAngle => coneAngle;
        public float LineWidth => lineWidth;
        public float RectangleWidth => rectangleWidth;
        public float RectangleLength => rectangleLength;

        public AbilityResourceType ResourceType => resourceType;
        public int ResourceCost => resourceCost;

        public float CastTime => castTime;
        public float Cooldown => cooldown;

        protected override bool ValidateData(out string errorMessage)
        {
            if (!ValidateRequiredString(abilityName, nameof(abilityName), out errorMessage))
                return false;

            if (!ValidateGreaterThanZero(range, nameof(range), out errorMessage))
                return false;

            if (!ValidateMinimumInt(resourceCost, 0, nameof(resourceCost), out errorMessage))
                return false;

            if (!ValidateMinimumFloat(castTime, 0f, nameof(castTime), out errorMessage))
                return false;

            if (!ValidateMinimumFloat(cooldown, 0f, nameof(cooldown), out errorMessage))
                return false;

            if (!ValidateAreaShape(out errorMessage))
                return false;

            errorMessage = string.Empty;
            return true;
        }

        private bool ValidateAreaShape(out string errorMessage)
        {
            switch (areaShape)
            {
                case AbilityAreaShape.SingleTarget:
                    errorMessage = string.Empty;
                    return true;

                case AbilityAreaShape.Circle:
                    return ValidateGreaterThanZero(areaRadius, nameof(areaRadius), out errorMessage);

                case AbilityAreaShape.Cone:
                    if (!ValidateGreaterThanZero(areaRadius, nameof(areaRadius), out errorMessage))
                        return false;

                    if (coneAngle <= 0f || coneAngle > 360f)
                    {
                        errorMessage = $"{nameof(coneAngle)} must be greater than 0 and less than or equal to 360. Current value: {coneAngle}.";
                        return false;
                    }

                    errorMessage = string.Empty;
                    return true;

                case AbilityAreaShape.Line:
                    if (!ValidateGreaterThanZero(areaRadius, nameof(areaRadius), out errorMessage))
                        return false;

                    if (!ValidateGreaterThanZero(lineWidth, nameof(lineWidth), out errorMessage))
                        return false;

                    errorMessage = string.Empty;
                    return true;

                case AbilityAreaShape.Rectangle:
                    if (!ValidateGreaterThanZero(rectangleWidth, nameof(rectangleWidth), out errorMessage))
                        return false;

                    if (!ValidateGreaterThanZero(rectangleLength, nameof(rectangleLength), out errorMessage))
                        return false;

                    errorMessage = string.Empty;
                    return true;

                default:
                    errorMessage = $"Unsupported area shape: {areaShape}.";
                    return false;
            }
        }
    }
}