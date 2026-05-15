using Skybound.Characters;
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

        public float RoundProgress => roundTimer / roundDuration;
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
            ResetCombatClock(0f);
        }

        private void Update()
        {
            if (CombatStateManager.Instance == null || !CombatStateManager.Instance.IsInCombat)
                return;

            TickRound();
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

            Debug.Log($"{name} started new round. Actions: {availableActions}, Main Attacks: {mainHandAttacksRemainingThisRound}, Offhand Attacks: {offHandAttacksRemainingThisRound}");
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
            Debug.Log($"{name} spent action. Actions left: {availableActions}");
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

            Debug.Log($"{name} spent main-hand attack. Main attacks left: {mainHandAttacksRemainingThisRound}");

            return true;
        }

        public bool TrySpendOffHandAttack()
        {
            if (offHandAttacksRemainingThisRound <= 0)
                return false;

            offHandAttacksRemainingThisRound--;
            attacksRemainingThisRound--;

            Debug.Log($"{name} spent off-hand attack. Offhand attacks left: {offHandAttacksRemainingThisRound}");

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