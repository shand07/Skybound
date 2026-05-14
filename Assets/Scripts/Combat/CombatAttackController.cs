using Skybound.Characters;
using Skybound.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Skybound.Combat
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CharacterStats))]
    [RequireComponent(typeof(CombatClock))]
    public class CombatAttackController : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private int baseWeaponDamage = 6;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackCheckInterval = 0.15f;
        [SerializeField] private float criticalDamageMultiplier = 2f;

        private NavMeshAgent agent;
        private CharacterStats attackerStats;
        private CombatClock combatClock;

        private CharacterStats currentTarget;
        private float attackCheckTimer;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            attackerStats = GetComponent<CharacterStats>();
            combatClock = GetComponent<CombatClock>();
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsPaused)
                return;

            if (currentTarget == null || currentTarget.IsDead)
                return;

            attackCheckTimer += Time.deltaTime;

            if (attackCheckTimer < attackCheckInterval)
                return;

            attackCheckTimer = 0f;
            HandleAttackTarget();
        }

        public void SetAttackTarget(CharacterStats target)
        {
            if (target == null || target == attackerStats)
                return;

            currentTarget = target;

            if (agent != null)
                agent.isStopped = false;

            CombatStateManager.Instance?.RegisterEnemy(target.gameObject);

            Debug.Log($"{name} attacking {target.name}");
        }

        public void ClearTarget()
        {
            currentTarget = null;
            
            if (agent != null)
                agent.isStopped = false;
        }

        private void HandleAttackTarget()
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distance > attackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(currentTarget.transform.position);
                return;
            }

            agent.isStopped = true;

            if (!combatClock.TrySpendAttack())
                return;

            PerformAttack();
        }

        private void PerformAttack()
        {
            bool hit = attackerStats.RollAttackAgainst(
                currentTarget,
                out int roll,
                out bool isCritical,
                out bool isNaturalMiss
            );

            if (!hit)
            {
                Debug.Log($"{name} rolled {roll} and missed {currentTarget.name}.");
                return;
            }

            int damage = baseWeaponDamage + attackerStats.GetMeleeDamageModifier();

            if (isCritical)
                damage = Mathf.RoundToInt(damage * criticalDamageMultiplier);

            Debug.Log($"{name} rolled {roll} and hit {currentTarget.name} for {damage} damage.");

            currentTarget.TakeDamage(damage);

            if (currentTarget.IsDead)
                ClearTarget();
        }
    }
}