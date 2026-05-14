using Skybound.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace Skybound.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CharacterMovementSpeed : MonoBehaviour
    {
        [Header("Movement Speeds")]
        [SerializeField] private float explorationSpeed = 6.5f;
        [SerializeField] private float combatSpeed = 3.5f;

        private NavMeshAgent agent;
        private bool isSubscribed;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            TrySubscribeToCombatState();
            ApplyCurrentSpeed();
        }

        private void OnDisable()
        {
            if (CombatStateManager.Instance != null && isSubscribed)
            {
                CombatStateManager.Instance.OnCombatStateChanged -= HandleCombatStateChanged;
                isSubscribed = false;
            }
        }

        private void TrySubscribeToCombatState()
        {
            if (CombatStateManager.Instance == null || isSubscribed)
                return;

            CombatStateManager.Instance.OnCombatStateChanged += HandleCombatStateChanged;
            isSubscribed = true;
        }

        private void ApplyCurrentSpeed()
        {
            bool isInCombat = CombatStateManager.Instance != null && CombatStateManager.Instance.IsInCombat;
            HandleCombatStateChanged(isInCombat);
        }

        private void HandleCombatStateChanged(bool isInCombat)
        {
            agent.speed = isInCombat ? combatSpeed : explorationSpeed;

            Debug.Log($"{name} speed changed to {agent.speed}");
        }
    }
}