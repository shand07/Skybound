using Skybound.Characters;
using UnityEngine;

namespace Skybound.UI
{
    public class MoveMarker : MonoBehaviour
    {
        [SerializeField] private float arriveDistance = 0.35f;

        private Transform targetUnit;
        private UnitMoveMarkerController owner;
        private Vector3 destination;

        public void Initialize(Transform unit, Vector3 moveDestination, UnitMoveMarkerController markerOwner)
        {
            targetUnit = unit;
            destination = moveDestination;
            owner = markerOwner;

            transform.position = destination;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (targetUnit == null)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 flatUnitPosition = targetUnit.position;
            flatUnitPosition.y = destination.y;

            float distance = Vector3.Distance(flatUnitPosition, destination);

            if (distance <= arriveDistance)
            {
                if (owner != null)
                    owner.ClearMarker();
                else
                    Destroy(gameObject);
            }
        }
    }
}