using System;
using System.Collections;
using System.Collections.Generic;
using Skybound.Core.Diagnostics;
using Skybound.Core.Services;
using UnityEngine;

namespace Skybound.Data.Json
{
    public class GameDataRepository : MonoBehaviour
    {
        public static GameDataRepository Instance { get; private set; }

        [Header("StreamingAssets Paths")]
        [SerializeField]
        private string characterDataPath =
            "Data/Characters/characters.json";

        private readonly Dictionary<string, CharacterDefinition>
            charactersById = new();

        public bool IsLoading { get; private set; }
        public bool IsLoaded { get; private set; }
        public bool HasLoadFailed { get; private set; }

        public int CharacterCount => charactersById.Count;

        public event Action OnDataLoaded;
        public event Action<string> OnDataLoadFailed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                SkyboundDebug.Warning(
                    "Duplicate GameDataRepository found. " +
                    "Destroying duplicate.",
                    this
                );

                Destroy(gameObject);
                return;
            }

            Instance = this;
            SkyboundServiceRegistry.Register(this);

            SkyboundDebug.Log(
                "GameDataRepository initialized.",
                this
            );

            StartCoroutine(LoadAllData());
        }

        private void OnDestroy()
        {
            if (Instance != this)
                return;

            Instance = null;

            SkyboundServiceRegistry
                .Unregister<GameDataRepository>();
        }

        public IEnumerator LoadAllData()
        {
            if (IsLoading)
            {
                SkyboundDebug.Warning(
                    "Game data is already loading.",
                    this
                );

                yield break;
            }

            IsLoading = true;
            IsLoaded = false;
            HasLoadFailed = false;

            charactersById.Clear();

            CharacterDefinitionCollection characterCollection = null;
            string characterLoadError = null;

            yield return JsonFileLoader
                .LoadStreamingAssetsJson<CharacterDefinitionCollection>(
                    characterDataPath,
                    loadedCollection =>
                    {
                        characterCollection = loadedCollection;
                    },
                    errorMessage =>
                    {
                        characterLoadError = errorMessage;
                    }
                );

            if (!string.IsNullOrWhiteSpace(characterLoadError))
            {
                FailLoading(characterLoadError);
                yield break;
            }

            if (!TryBuildCharacterDatabase(
                    characterCollection,
                    out string validationError))
            {
                FailLoading(validationError);
                yield break;
            }

            IsLoading = false;
            IsLoaded = true;
            HasLoadFailed = false;

            SkyboundDebug.Log(
                $"Game data loaded successfully. " +
                $"Characters: {charactersById.Count}.",
                this
            );

            OnDataLoaded?.Invoke();
        }

        public bool TryGetCharacter(
            string characterId,
            out CharacterDefinition character)
        {
            character = null;

            if (!IsLoaded)
            {
                SkyboundDebug.Warning(
                    $"Cannot retrieve character '{characterId}' because " +
                    "GameDataRepository has not finished loading.",
                    this
                );

                return false;
            }

            if (string.IsNullOrWhiteSpace(characterId))
            {
                SkyboundDebug.Warning(
                    "GameDataRepository received an empty character id.",
                    this
                );

                return false;
            }

            if (!charactersById.TryGetValue(
                    characterId,
                    out character))
            {
                SkyboundDebug.Warning(
                    $"Character definition '{characterId}' was not found.",
                    this
                );

                return false;
            }

            return true;
        }

        public CharacterDefinition GetCharacter(string characterId)
        {
            if (TryGetCharacter(
                    characterId,
                    out CharacterDefinition character))
            {
                return character;
            }

            return null;
        }

        public bool ContainsCharacter(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
                return false;

            return charactersById.ContainsKey(characterId);
        }

        private bool TryBuildCharacterDatabase(
            CharacterDefinitionCollection collection,
            out string errorMessage)
        {
            if (collection == null)
            {
                errorMessage =
                    "Character definition collection was null.";

                return false;
            }

            if (collection.characters == null ||
                collection.characters.Length == 0)
            {
                errorMessage =
                    $"Character data file '{characterDataPath}' " +
                    "contains no characters.";

                return false;
            }

            Dictionary<string, CharacterDefinition>
                validatedCharacters = new();

            for (int i = 0;
                 i < collection.characters.Length;
                 i++)
            {
                CharacterDefinition character =
                    collection.characters[i];

                if (character == null)
                {
                    errorMessage =
                        $"Character entry at index {i} is null.";

                    return false;
                }

                if (!character.IsValid(
                        out string characterError))
                {
                    errorMessage =
                        $"Character entry at index {i} is invalid: " +
                        characterError;

                    return false;
                }

                if (validatedCharacters.ContainsKey(character.Id))
                {
                    errorMessage =
                        $"Duplicate character id '{character.Id}' found.";

                    return false;
                }

                validatedCharacters.Add(
                    character.Id,
                    character
                );
            }

            charactersById.Clear();

            foreach (
                KeyValuePair<string, CharacterDefinition> entry
                in validatedCharacters)
            {
                charactersById.Add(
                    entry.Key,
                    entry.Value
                );
            }

            errorMessage = string.Empty;
            return true;
        }

        private void FailLoading(string errorMessage)
        {
            IsLoading = false;
            IsLoaded = false;
            HasLoadFailed = true;

            charactersById.Clear();

            SkyboundDebug.Error(
                $"Game data loading failed. {errorMessage}",
                this
            );

            OnDataLoadFailed?.Invoke(errorMessage);
        }
    }
}