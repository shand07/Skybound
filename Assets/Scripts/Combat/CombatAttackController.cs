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
        private enum AttackHand
        {
            MainHand,
            OffHand
        }

        [Header("Fallback Attack Settings")]
        [SerializeField] private int baseWeaponDamage = 6;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackCheckInterval = 0.15f;
        [SerializeField] private float criticalDamageMultiplier = 2f;

        [Header("Dual Wield")]
        [SerializeField] private float offHandFollowUpDelay = 0.25f;
        [SerializeField] private int offHandAccuracyPenalty = -4;

        private NavMeshAgent agent;
        private CharacterStats attackerStats;
        private CombatClock combatClock;
        private CharacterEquipment equipment;

        private CharacterStats currentTarget;
        private float attackCheckTimer;

        private bool hasPendingOffHandAttack;
        private float pendingOffHandTimer;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            attackerStats = GetComponent<CharacterStats>();
            combatClock = GetComponent<CombatClock>();
            equipment = GetComponent<CharacterEquipment>();

            UpdateStoppingDistance();
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsPaused)
                return;

            if (currentTarget == null || currentTarget.IsDead)
                return;

            HandlePendingOffHandAttack();

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

            UpdateStoppingDistance();

            if (agent != null)
                agent.isStopped = false;

            CombatStateManager.Instance?.RegisterEnemy(target.gameObject);

            Debug.Log($"{name} attacking {target.name}");
        }

        public void ClearTarget()
        {
            currentTarget = null;
            hasPendingOffHandAttack = false;
            pendingOffHandTimer = 0f;

            if (agent != null)
                agent.isStopped = false;
        }

        private void HandleAttackTarget()
        {
            float currentAttackRange = GetAttackRange();

            float distance = Vector3.Distance(
                transform.position,
                currentTarget.transform.position
            );

            if (distance > currentAttackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(currentTarget.transform.position);
                return;
            }

            agent.isStopped = true;

            if (!combatClock.TrySpendMainHandAttack())
                return;

            PerformAttack(AttackHand.MainHand);

            if (equipment != null && equipment.HasOffHandAttack())
            {
                hasPendingOffHandAttack = true;
                pendingOffHandTimer = offHandFollowUpDelay;
            }
        }

        private void HandlePendingOffHandAttack()
        {
            if (!hasPendingOffHandAttack)
                return;

            pendingOffHandTimer -= Time.deltaTime;

            if (pendingOffHandTimer > 0f)
                return;

            hasPendingOffHandAttack = false;

            if (currentTarget == null || currentTarget.IsDead)
                return;

            float distance = Vector3.Distance(
                transform.position,
                currentTarget.transform.position
            );

            if (distance > GetAttackRange())
                return;

            if (!combatClock.TrySpendOffHandAttack())
                return;

            PerformAttack(AttackHand.OffHand);
        }

        private void PerformAttack(AttackHand attackHand)
        {
            int accuracyBonus = GetAccuracyBonus(attackHand);

            bool hit = attackerStats.RollAttackAgainst(
                currentTarget,
                accuracyBonus,
                out int roll,
                out bool isCritical,
                out bool isNaturalMiss
            );

            if (!hit)
            {
                Debug.Log($"{name} rolled {roll} and missed {currentTarget.name} with {attackHand}.");
                return;
            }

            int damage = GetWeaponDamage(attackHand) + attackerStats.GetMeleeDamageModifier();

            if (isCritical)
                damage = Mathf.RoundToInt(damage * criticalDamageMultiplier);

            Debug.Log($"{name} rolled {roll} and hit {currentTarget.name} with {attackHand} for {damage} damage.");

            currentTarget.TakeDamage(damage);

            if (currentTarget.IsDead)
                ClearTarget();
        }

        private int GetWeaponDamage(AttackHand attackHand)
        {
            if (equipment == null)
                return baseWeaponDamage;

            if (attackHand == AttackHand.OffHand)
                return equipment.GetOffHandDamage();

            return equipment.GetMainHandDamage();
        }

        private int GetAccuracyBonus(AttackHand attackHand)
        {
            if (equipment == null)
                return attackHand == AttackHand.OffHand ? offHandAccuracyPenalty : 0;

            if (attackHand == AttackHand.OffHand)
                return equipment.GetOffHandAccuracyBonus() + offHandAccuracyPenalty;

            return equipment.GetMainHandAccuracyBonus();
        }

        private float GetAttackRange()
        {
            if (equipment != null)
                return equipment.GetMainHandAttackRange();

            return attackRange;
        }

        private void UpdateStoppingDistance()
        {
            if (agent == null)
                return;

            agent.stoppingDistance = GetAttackRange() * 0.8f;
        }
    }
}