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
        }

        public bool CanBeSelectedByPlayer()
        {
            return identity != null && identity.CanBeSelectedByPlayer();
        }

        public void Select()
        {
            if (!CanBeSelectedByPlayer())
                return;

            IsSelected = true;
        }

        public void Deselect()
        {
            IsSelected = false;
        }

        public void MoveTo(Vector3 destination)
        {
            if (!CanBeSelectedByPlayer())
                return;

            agent.stoppingDistance = 0.1f;
            agent.isStopped = false;
            agent.SetDestination(destination);
        }
    }
}