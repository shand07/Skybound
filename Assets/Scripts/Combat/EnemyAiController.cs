using Skybound.Characters;
using UnityEngine;

namespace Skybound.Combat
{
    [RequireComponent(typeof(UnitIdentity))]
    [RequireComponent(typeof(CombatAttackController))]
    public class EnemyAIController : MonoBehaviour
    {
        private UnitIdentity unitIdentity;
        private CombatAttackController attackController;
        private CharacterStats currentTarget;

        public bool HasTarget => currentTarget != null && !currentTarget.IsDead;

        private void Awake()
        {
            unitIdentity = GetComponent<UnitIdentity>();
            attackController = GetComponent<CombatAttackController>();
        }

        private void Update()
        {
            if (currentTarget == null || currentTarget.IsDead)
                return;

            attackController.SetAttackTarget(currentTarget);
        }

        public bool CanAggro()
        {
            return unitIdentity != null &&
                   unitIdentity.IsHostileToPlayer() &&
                   !HasTarget;
        }

        public void Aggro(CharacterStats target)
        {
            if (target == null || target.IsDead)
                return;

            currentTarget = target;

            CombatStateManager.Instance?.RegisterEnemy(gameObject);
            attackController.SetAttackTarget(currentTarget);

            Debug.Log($"{name} aggroed onto {target.name}");
        }

        public void ClearTarget()
        {
            currentTarget = null;

            if (attackController != null)
                attackController.ClearTarget();
        }
    }
}