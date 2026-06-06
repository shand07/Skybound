using Skybound.Combat;
using Skybound.Core.Diagnostics;
using Skybound.Core.Services;
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
        private CombatStateManager combatStateManager;
        private bool isSubscribed;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();

            if (agent == null)
                SkyboundDebug.MissingReference(this, nameof(NavMeshAgent));
        }

        private void Start()
        {
            ResolveDependencies();
            SubscribeToCombatState();
            ApplyCurrentSpeed();
        }

        private void OnDisable()
        {
            UnsubscribeFromCombatState();
        }

        private void ResolveDependencies()
        {
            if (!SkyboundServiceRegistry.TryGet(out combatStateManager))
            {
                SkyboundDebug.ServiceUnavailable(
                    this,
                    nameof(CombatStateManager),
                    "Make sure CombatStateManager exists in the scene and registers itself in Awake."
                );
            }
        }

        private void SubscribeToCombatState()
        {
            if (combatStateManager == null)
                return;

            if (isSubscribed)
                return;

            combatStateManager.OnCombatStateChanged += HandleCombatStateChanged;
            isSubscribed = true;

            SkyboundDebug.Log($"{name} subscribed to combat state changes.", this);
        }

        private void UnsubscribeFromCombatState()
        {
            if (combatStateManager == null || !isSubscribed)
                return;

            combatStateManager.OnCombatStateChanged -= HandleCombatStateChanged;
            isSubscribed = false;

            SkyboundDebug.Log($"{name} unsubscribed from combat state changes.", this);
        }

        private void ApplyCurrentSpeed()
        {
            if (combatStateManager == null)
                return;

            HandleCombatStateChanged(combatStateManager.IsInCombat);
        }

        private void HandleCombatStateChanged(bool isInCombat)
        {
            if (agent == null)
                return;

            agent.speed = isInCombat ? combatSpeed : explorationSpeed;

            SkyboundDebug.Log($"{name} speed changed to {agent.speed}.", this);
        }
    }
}