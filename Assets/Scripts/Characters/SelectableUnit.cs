using Skybound.Core.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

namespace Skybound.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(UnitIdentity))]
    public class SelectableUnit : MonoBehaviour
    {
        private NavMeshAgent agent;
        private UnitIdentity identity;

        public bool IsSelected { get; private set; }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            identity = GetComponent<UnitIdentity>();

            ValidateReferences();
        }

        private void ValidateReferences()
        {
            if (agent == null)
                SkyboundDebug.MissingReference(this, nameof(NavMeshAgent));

            if (identity == null)
                SkyboundDebug.MissingReference(this, nameof(UnitIdentity));
        }

        public bool CanBeSelectedByPlayer()
        {
            if (identity == null)
            {
                SkyboundDebug.Warning($"{name} cannot be selected because UnitIdentity is missing.", this);
                return false;
            }

            return identity.CanBeSelectedByPlayer();
        }

        public void Select()
        {
            if (!CanBeSelectedByPlayer())
            {
                SkyboundDebug.Warning($"{name} selection failed.", this);
                return;
            }

            IsSelected = true;
            SkyboundDebug.Log($"{name} selected.", this);
        }

        public void Deselect()
        {
            if (!IsSelected)
                return;

            IsSelected = false;
            SkyboundDebug.Log($"{name} deselected.", this);
        }

        public void MoveTo(Vector3 destination)
        {
            if (!CanBeSelectedByPlayer())
            {
                SkyboundDebug.Warning($"{name} cannot move because it is not player-selectable.", this);
                return;
            }

            if (agent == null)
            {
                SkyboundDebug.MissingReference(this, nameof(NavMeshAgent), "Movement command failed.");
                return;
            }

            if (!agent.isOnNavMesh)
            {
                SkyboundDebug.Warning($"{name} cannot move because its NavMeshAgent is not on a NavMesh.", this);
                return;
            }

            agent.stoppingDistance = 0.1f;
            agent.isStopped = false;
            agent.SetDestination(destination);

            SkyboundDebug.Log($"{name} moving to {destination}.", this);
        }
    }
}