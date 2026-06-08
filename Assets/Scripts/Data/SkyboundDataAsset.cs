using Skybound.Core.Diagnostics;
using UnityEngine;

namespace Skybound.Data
{
    public abstract class SkyboundDataAsset : ScriptableObject
    {
        public bool IsValid(out string errorMessage)
        {
            return ValidateData(out errorMessage);
        }

        protected abstract bool ValidateData(out string errorMessage);

        protected virtual void OnValidate()
        {
            if (!ValidateData(out string errorMessage))
            {
                SkyboundDebug.Warning(
                    $"{GetType().Name} asset '{name}' is invalid: {errorMessage}",
                    this
                );
            }
        }

        protected bool ValidateRequiredString(
            string value,
            string fieldName,
            out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errorMessage = $"{fieldName} is required.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        protected bool ValidateMinimumInt(
            int value,
            int minimum,
            string fieldName,
            out string errorMessage)
        {
            if (value < minimum)
            {
                errorMessage = $"{fieldName} must be at least {minimum}. Current value: {value}.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        protected bool ValidateMinimumFloat(
            float value,
            float minimum,
            string fieldName,
            out string errorMessage)
        {
            if (value < minimum)
            {
                errorMessage = $"{fieldName} must be at least {minimum}. Current value: {value}.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        protected bool ValidateGreaterThanZero(
            float value,
            string fieldName,
            out string errorMessage)
        {
            if (value <= 0f)
            {
                errorMessage = $"{fieldName} must be greater than 0. Current value: {value}.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        protected bool ValidateGreaterThanZero(
            int value,
            string fieldName,
            out string errorMessage)
        {
            if (value <= 0)
            {
                errorMessage = $"{fieldName} must be greater than 0. Current value: {value}.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}