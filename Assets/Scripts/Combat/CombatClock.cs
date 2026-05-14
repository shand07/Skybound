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
        [SerializeField] private float nextAttackTime;

        public float RoundProgress => roundTimer / roundDuration;
        public int AvailableActions => availableActions;
        public int AttacksRemainingThisRound => attacksRemainingThisRound;
        public float RoundDuration => roundDuration;
        public float AttacksPerRound => attacksPerRound;

        private void Awake()
        {
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
            attacksRemainingThisRound = Mathf.FloorToInt(attacksPerRound);

            nextAttackTime = 0f;
        }

        private void TickRound()
        {
            roundTimer += Time.deltaTime;

            if (roundTimer >= roundDuration)
            {
                StartNewRound();
            }
        }

        private void StartNewRound()
        {
            roundTimer = 0f;

            availableActions = actionsPerRound;
            attacksRemainingThisRound = Mathf.FloorToInt(attacksPerRound);

            nextAttackTime = 0f;

            Debug.Log($"{name} started new round. Actions: {availableActions}, Attacks: {attacksRemainingThisRound}");
        }

        public bool TrySpendAction()
        {
            if (availableActions <= 0)
                return false;

            availableActions--;

            Debug.Log($"{name} spent action. Actions left: {availableActions}");
            return true;
        }

        public bool TrySpendAttack()
        {
            if (attacksRemainingThisRound <= 0)
                return false;

            if (roundTimer < nextAttackTime)
                return false;

            attacksRemainingThisRound--;

            float attackInterval = attacksPerRound > 0f
                ? roundDuration / attacksPerRound
                : roundDuration;

            nextAttackTime += attackInterval;

            Debug.Log($"{name} spent attack. Attacks left: {attacksRemainingThisRound}");

            return true;
        }
    }
}