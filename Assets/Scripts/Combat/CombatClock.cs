using Skybound.Characters;
using Skybound.Core.Diagnostics;
using Skybound.Core.Services;
using UnityEngine;

namespace Skybound.Combat
{
    public class CombatClock : MonoBehaviour
    {
        [Header("Round Rules")]
        [SerializeField] private float roundDuration = 6f;

        [Header("Action Economy")]
        [SerializeField] private int actionsPerRound = 1;
        [SerializeField] private float attacksPerRound = 1f;

        [Header("Runtime Debug")]
        [SerializeField] private float roundTimer;
        [SerializeField] private int availableActions;
        [SerializeField] private int attacksRemainingThisRound;
        [SerializeField] private int mainHandAttacksRemainingThisRound;
        [SerializeField] private int offHandAttacksRemainingThisRound;
        [SerializeField] private float nextMainHandAttackTime;

        private CharacterEquipment equipment;
        private CombatStateManager combatStateManager;

        public float RoundProgress => roundDuration > 0f ? roundTimer / roundDuration : 0f;
        public int AvailableActions => availableActions;
        public int AttacksRemainingThisRound => attacksRemainingThisRound;
        public int MainHandAttacksRemainingThisRound => mainHandAttacksRemainingThisRound;
        public int OffHandAttacksRemainingThisRound => offHandAttacksRemainingThisRound;
        public float RoundDuration => roundDuration;
        public float BaseAttacksPerRound => attacksPerRound;
        public float TotalDisplayedAttacksPerRound => GetTotalDisplayedAttacksPerRound();

        private void Awake()
        {
            equipment = GetComponent<CharacterEquipment>();

            if (roundDuration <= 0f)
            {
                SkyboundDebug.Warning($"{name} had invalid roundDuration. Resetting to 6.", this);
                roundDuration = 6f;
            }

            if (actionsPerRound < 0)
            {
                SkyboundDebug.Warning($"{name} had negative actionsPerRound. Resetting to 0.", this);
                actionsPerRound = 0;
            }

            if (attacksPerRound < 0f)
            {
                SkyboundDebug.Warning($"{name} had negative attacksPerRound. Resetting to 0.", this);
                attacksPerRound = 0f;
            }

            ResetCombatClock(0f);
        }

        private void Start()
        {
            ResolveDependencies();
        }

        private void Update()
        {
            if (combatStateManager == null)
                return;

            if (!combatStateManager.IsInCombat)
                return;

            TickRound();
        }

        private void ResolveDependencies()
        {
            if (!SkyboundServiceRegistry.TryGet(out combatStateManager))
            {
                SkyboundDebug.ServiceUnavailable(
                    this,
                    nameof(CombatStateManager),
                    "CombatClock will not tick until CombatStateManager is available."
                );
            }
        }

        public void ResetCombatClock(float startingProgress)
        {
            roundTimer = Mathf.Clamp01(startingProgress) * roundDuration;
            availableActions = actionsPerRound;
            ResetRoundAttacks();
            nextMainHandAttackTime = 0f;
        }

        private void TickRound()
        {
            roundTimer += Time.deltaTime;

            if (roundTimer >= roundDuration)
                StartNewRound();
        }

        private void StartNewRound()
        {
            roundTimer = 0f;
            availableActions = actionsPerRound;
            ResetRoundAttacks();
            nextMainHandAttackTime = 0f;

            SkyboundDebug.Log(
                $"{name} started new round. Actions: {availableActions}, Main Attacks: {mainHandAttacksRemainingThisRound}, Offhand Attacks: {offHandAttacksRemainingThisRound}",
                this
            );
        }

        private void ResetRoundAttacks()
        {
            float mainHandAPR = GetMainHandAttacksPerRound();
            float offHandAPR = GetOffHandAttacksPerRound();

            mainHandAttacksRemainingThisRound = Mathf.FloorToInt(mainHandAPR);
            offHandAttacksRemainingThisRound = Mathf.FloorToInt(offHandAPR);

            attacksRemainingThisRound =
                mainHandAttacksRemainingThisRound +
                offHandAttacksRemainingThisRound;
        }

        public bool TrySpendAction()
        {
            if (availableActions <= 0)
                return false;

            availableActions--;

            SkyboundDebug.Log($"{name} spent action. Actions left: {availableActions}", this);
            return true;
        }

        public bool TrySpendMainHandAttack()
        {
            if (mainHandAttacksRemainingThisRound <= 0)
                return false;

            if (roundTimer < nextMainHandAttackTime)
                return false;

            mainHandAttacksRemainingThisRound--;
            attacksRemainingThisRound--;

            float mainHandAPR = GetMainHandAttacksPerRound();

            float attackInterval = mainHandAPR > 0f
                ? roundDuration / mainHandAPR
                : roundDuration;

            nextMainHandAttackTime += attackInterval;

            SkyboundDebug.Log($"{name} spent main-hand attack. Main attacks left: {mainHandAttacksRemainingThisRound}", this);

            return true;
        }

        public bool TrySpendOffHandAttack()
        {
            if (offHandAttacksRemainingThisRound <= 0)
                return false;

            offHandAttacksRemainingThisRound--;
            attacksRemainingThisRound--;

            SkyboundDebug.Log($"{name} spent off-hand attack. Offhand attacks left: {offHandAttacksRemainingThisRound}", this);

            return true;
        }

        private float GetMainHandAttacksPerRound()
        {
            float total = attacksPerRound;

            if (equipment != null)
                total += equipment.GetMainHandAttacksPerRoundBonus();

            return Mathf.Max(0f, total);
        }

        private float GetOffHandAttacksPerRound()
        {
            if (equipment == null || !equipment.HasOffHandAttack())
                return 0f;

            return 1f + equipment.GetOffHandPowerAttacksPerRoundBonus();
        }

        private float GetTotalDisplayedAttacksPerRound()
        {
            return GetMainHandAttacksPerRound() + GetOffHandAttacksPerRound();
        }
    }
}