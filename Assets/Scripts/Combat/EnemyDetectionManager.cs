using Skybound.Characters;
using Skybound.Core.Diagnostics;
using Skybound.Core.Services;
using Skybound.Systems.FogOfWar;
using UnityEngine;

namespace Skybound.Combat
{
    public class EnemyDetectionManager : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] private float detectionRange = 10f;

        [Header("Fog Rules")]
        [SerializeField] private bool requireFogVisibility = true;

        private FogOfWarManager fogOfWarManager;

        private void Start()
        {
            ResolveDependencies();
            ValidateSettings();
        }

        private void Update()
        {
            CheckEnemyAggro();
        }

        private void ResolveDependencies()
        {
            if (!SkyboundServiceRegistry.TryGet(out fogOfWarManager))
            {
                if (requireFogVisibility)
                {
                    SkyboundDebug.ServiceUnavailable(
                        this,
                        nameof(FogOfWarManager),
                        "Enemy detection requires fog visibility, so enemies will not aggro from fog."
                    );
                }
                else
                {
                    SkyboundDebug.Warning(
                        "FogOfWarManager not found. Enemy detection will ignore fog visibility.",
                        this
                    );
                }
            }
        }

        private void ValidateSettings()
        {
            if (detectionRange <= 0f)
            {
                SkyboundDebug.Warning($"{name} had invalid detectionRange. Resetting to 10.", this);
                detectionRange = 10f;
            }
        }

        private void CheckEnemyAggro()
        {
            EnemyAIController[] enemies =
                FindObjectsByType<EnemyAIController>(FindObjectsSortMode.None);

            CharacterStats[] characters =
                FindObjectsByType<CharacterStats>(FindObjectsSortMode.None);

            foreach (EnemyAIController enemy in enemies)
            {
                if (enemy == null || !enemy.CanAggro())
                    continue;

                if (!IsEnemyVisibleToPlayer(enemy.transform.position))
                    continue;

                CharacterStats nearestPlayer = FindNearestPlayer(enemy.transform.position, characters);

                if (nearestPlayer == null)
                    continue;

                float distance = Vector3.Distance(enemy.transform.position, nearestPlayer.transform.position);

                if (distance <= detectionRange)
                    enemy.Aggro(nearestPlayer);
            }
        }

        private CharacterStats FindNearestPlayer(Vector3 enemyPosition, CharacterStats[] characters)
        {
            CharacterStats nearestPlayer = null;
            float nearestDistance = float.MaxValue;

            foreach (CharacterStats character in characters)
            {
                if (character == null || character.IsDead)
                    continue;

                UnitIdentity identity = character.GetComponent<UnitIdentity>();

                if (identity == null || !identity.CanBeSelectedByPlayer())
                    continue;

                float distance = Vector3.Distance(enemyPosition, character.transform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlayer = character;
                }
            }

            return nearestPlayer;
        }

        private bool IsEnemyVisibleToPlayer(Vector3 enemyPosition)
        {
            if (fogOfWarManager == null)
                return !requireFogVisibility;

            return fogOfWarManager.GetFogStateAtWorldPosition(enemyPosition) == FogState.Visible;
        }
    }
}