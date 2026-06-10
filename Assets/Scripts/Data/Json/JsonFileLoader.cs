using System;
using System.Collections;
using System.IO;
using Skybound.Core.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

namespace Skybound.Data.Json
{
    public static class JsonFileLoader
    {
        public static IEnumerator LoadStreamingAssetsJson<T>(
            string relativePath,
            Action<T> onSuccess,
            Action<string> onFailure)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                const string errorMessage =
                    "JsonFileLoader received an empty relative path.";

                SkyboundDebug.Error(errorMessage);
                onFailure?.Invoke(errorMessage);

                yield break;
            }

            string fullPath = Path.Combine(
                Application.streamingAssetsPath,
                relativePath
            );

            string requestPath = ConvertToRequestPath(fullPath);

            SkyboundDebug.Log(
                $"Loading JSON data from '{relativePath}'."
            );

            using UnityWebRequest request =
                UnityWebRequest.Get(requestPath);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                string errorMessage =
                    $"Failed to load JSON file '{relativePath}'. " +
                    $"Request error: {request.error}";

                SkyboundDebug.Error(errorMessage);
                onFailure?.Invoke(errorMessage);

                yield break;
            }

            string json = request.downloadHandler.text;

            if (string.IsNullOrWhiteSpace(json))
            {
                string errorMessage =
                    $"JSON file '{relativePath}' was empty.";

                SkyboundDebug.Error(errorMessage);
                onFailure?.Invoke(errorMessage);

                yield break;
            }

            T deserializedData;

            try
            {
                deserializedData = JsonUtility.FromJson<T>(json);
            }
            catch (Exception exception)
            {
                string errorMessage =
                    $"Failed to deserialize JSON file '{relativePath}'. " +
                    $"Exception: {exception.Message}";

                SkyboundDebug.Error(errorMessage);
                onFailure?.Invoke(errorMessage);

                yield break;
            }

            if (deserializedData == null)
            {
                string errorMessage =
                    $"Deserializing JSON file '{relativePath}' returned null.";

                SkyboundDebug.Error(errorMessage);
                onFailure?.Invoke(errorMessage);

                yield break;
            }

            SkyboundDebug.Log(
                $"Successfully loaded JSON file '{relativePath}'."
            );

            onSuccess?.Invoke(deserializedData);
        }

        private static string ConvertToRequestPath(string fullPath)
        {
            string normalizedPath =
                fullPath.Replace("\\", "/");

            if (normalizedPath.Contains("://"))
                return normalizedPath;

            return $"file:///{normalizedPath}";
        }
    }
}