using Skybound.Characters;
using Skybound.Systems.FogOfWar;
using UnityEngine;

namespace Skybound.Combat
{
    public class EnemyDetectionManager : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] private float detectionRange = 10f;

        private void Update()
        {
            CheckEnemyAggro();
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
                {
                    enemy.Aggro(nearestPlayer);
                }
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
            if (FogOfWarManager.Instance == null)
                return true;

            return FogOfWarManager.Instance.GetFogStateAtWorldPosition(enemyPosition) == FogState.Visible;
        }
    }
}