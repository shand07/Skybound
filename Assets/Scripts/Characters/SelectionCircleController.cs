using UnityEngine;

namespace Skybound.Characters
{
    public class SelectionCircleController : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] private Material partySelectedMaterial;
        [SerializeField] private Material partyUnselectedMaterial;
        [SerializeField] private Material neutralMaterial;
        [SerializeField] private Material enemyMaterial;

        [Header("References")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private SelectableUnit selectableUnit;
        [SerializeField] private UnitIdentity unitIdentity;

        private void Awake()
        {
            if (lineRenderer == null)
                lineRenderer = GetComponent<LineRenderer>();

            if (selectableUnit == null)
                selectableUnit = GetComponentInParent<SelectableUnit>();

            if (unitIdentity == null)
                unitIdentity = GetComponentInParent<UnitIdentity>();
        }

        private void Update()
        {
            UpdateCircleColor();
        }

        private void UpdateCircleColor()
        {
            if (unitIdentity == null || lineRenderer == null)
                return;

            switch (unitIdentity.Faction)
            {
                case UnitFaction.PlayerParty:

                    if (selectableUnit != null && selectableUnit.IsSelected)
                    {
                        lineRenderer.material = partySelectedMaterial;
                    }
                    else
                    {
                        lineRenderer.material = partyUnselectedMaterial;
                    }

                    break;

                case UnitFaction.AllyGuest:
                case UnitFaction.Neutral:

                    lineRenderer.material = neutralMaterial;
                    break;

                case UnitFaction.Enemy:

                    lineRenderer.material = enemyMaterial;
                    break;
            }
        }
    }
}