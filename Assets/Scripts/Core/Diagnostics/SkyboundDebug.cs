using UnityEngine;

namespace Skybound.Core.Diagnostics
{
    public static class SkyboundDebug
    {
        private const string Prefix = "[Skybound]";

        public static void Log(string message, Object context = null)
        {
            Debug.Log($"{Prefix} {message}", context);
        }

        public static void Warning(string message, Object context = null)
        {
            Debug.LogWarning($"{Prefix} {message}", context);
        }

        public static void Error(string message, Object context = null)
        {
            Debug.LogError($"{Prefix} {message}", context);
        }

        public static void MissingReference(
            Object owner,
            string missingDependency,
            string suggestion = null)
        {
            string ownerName = owner != null ? owner.name : "Unknown Owner";

            string message =
                $"{ownerName} is missing required dependency: {missingDependency}.";

            if (!string.IsNullOrWhiteSpace(suggestion))
                message += $" {suggestion}";

            Error(message, owner);
        }

        public static void ServiceUnavailable(
            Object owner,
            string serviceName,
            string suggestion = null)
        {
            string ownerName = owner != null ? owner.name : "Unknown Owner";

            string message =
                $"{ownerName} could not resolve service: {serviceName}.";

            if (!string.IsNullOrWhiteSpace(suggestion))
                message += $" {suggestion}";

            Error(message, owner);
        }
    }
}