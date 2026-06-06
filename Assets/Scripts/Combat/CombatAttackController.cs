using Skybound.Characters;
using Skybound.Core;
using Skybound.Core.Diagnostics;
using Skybound.Core.Services;
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
        private GameManager gameManager;
        private CombatStateManager combatStateManager;

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

            ValidateReferences();
            ValidateSettings();
            UpdateStoppingDistance();
        }

        private void Start()
        {
            ResolveDependencies();
        }

        private void Update()
        {
            if (gameManager != null && gameManager.IsPaused)
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

        private void ResolveDependencies()
        {
            if (!SkyboundServiceRegistry.TryGet(out gameManager))
            {
                SkyboundDebug.ServiceUnavailable(
                    this,
                    nameof(GameManager),
                    "CombatAttackController will ignore pause state until GameManager is available."
                );
            }

            if (!SkyboundServiceRegistry.TryGet(out combatStateManager))
            {
                SkyboundDebug.ServiceUnavailable(
                    this,
                    nameof(CombatStateManager),
                    "CombatAttackController cannot register combat targets without CombatStateManager."
                );
            }
        }

        private void ValidateReferences()
        {
            if (agent == null)
                SkyboundDebug.MissingReference(this, nameof(NavMeshAgent));

            if (attackerStats == null)
                SkyboundDebug.MissingReference(this, nameof(CharacterStats));

            if (combatClock == null)
                SkyboundDebug.MissingReference(this, nameof(CombatClock));
        }

        private void ValidateSettings()
        {
            if (baseWeaponDamage < 0)
            {
                SkyboundDebug.Warning($"{name} had negative baseWeaponDamage. Resetting to 0.", this);
                baseWeaponDamage = 0;
            }

            if (attackRange <= 0f)
            {
                SkyboundDebug.Warning($"{name} had invalid attackRange. Resetting to 2.", this);
                attackRange = 2f;
            }

            if (attackCheckInterval <= 0f)
            {
                SkyboundDebug.Warning($"{name} had invalid attackCheckInterval. Resetting to 0.15.", this);
                attackCheckInterval = 0.15f;
            }

            if (criticalDamageMultiplier < 1f)
            {
                SkyboundDebug.Warning($"{name} had invalid criticalDamageMultiplier. Resetting to 2.", this);
                criticalDamageMultiplier = 2f;
            }

            if (offHandFollowUpDelay < 0f)
            {
                SkyboundDebug.Warning($"{name} had negative offHandFollowUpDelay. Resetting to 0.", this);
                offHandFollowUpDelay = 0f;
            }
        }

        public void SetAttackTarget(CharacterStats target)
        {
            if (target == null)
            {
                SkyboundDebug.Warning($"{name} tried to attack a null target.", this);
                return;
            }

            if (target == attackerStats)
            {
                SkyboundDebug.Warning($"{name} tried to attack itself.", this);
                return;
            }

            currentTarget = target;

            UpdateStoppingDistance();

            if (agent != null)
                agent.isStopped = false;

            if (combatStateManager != null)
                combatStateManager.RegisterEnemy(target.gameObject);

            SkyboundDebug.Log($"{name} attacking {target.name}.", this);
        }

        public void ClearTarget()
        {
            if (currentTarget != null)
                SkyboundDebug.Log($"{name} cleared attack target {currentTarget.name}.", this);

            currentTarget = null;
            hasPendingOffHandAttack = false;
            pendingOffHandTimer = 0f;

            if (agent != null)
                agent.isStopped = false;
        }

        private void HandleAttackTarget()
        {
            if (currentTarget == null || currentTarget.IsDead)
                return;

            float currentAttackRange = GetAttackRange();

            float distance = Vector3.Distance(
                transform.position,
                currentTarget.transform.position
            );

            if (distance > currentAttackRange)
            {
                if (agent == null)
                    return;

                agent.isStopped = false;
                agent.SetDestination(currentTarget.transform.position);
                return;
            }

            if (agent != null)
                agent.isStopped = true;

            if (combatClock == null || !combatClock.TrySpendMainHandAttack())
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

            if (combatClock == null || !combatClock.TrySpendOffHandAttack())
                return;

            PerformAttack(AttackHand.OffHand);
        }

        private void PerformAttack(AttackHand attackHand)
        {
            if (attackerStats == null || currentTarget == null)
                return;

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
                SkyboundDebug.Log($"{name} rolled {roll} and missed {currentTarget.name} with {attackHand}.", this);
                return;
            }

            int damage = GetWeaponDamage(attackHand) + attackerStats.GetMeleeDamageModifier();

            if (isCritical)
                damage = Mathf.RoundToInt(damage * criticalDamageMultiplier);

            SkyboundDebug.Log($"{name} rolled {roll} and hit {currentTarget.name} with {attackHand} for {damage} damage.", this);

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