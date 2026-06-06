using System.Collections.Generic;
using Skybound.Characters;
using Skybound.Combat;
using Skybound.Core;
using Skybound.Core.Diagnostics;
using Skybound.UI;
using UnityEngine;

namespace Skybound.InputSystem
{
    public class MoveCommandSystem : MonoBehaviour
    {
        [Header("Formation")]
        [SerializeField] private FormationType currentFormation = FormationType.Grid;
        [SerializeField] private float formationSpacing = 1.5f;
        [SerializeField] private int formationColumns = 3;

        [Header("Move Markers")]
        [SerializeField] private MoveMarker moveMarkerPrefab;
        [SerializeField] private float markerGroundOffset = 0.05f;

        public FormationType CurrentFormation => currentFormation;

        private void Awake()
        {
            ValidateSettings();
        }

        public void SetFormation(FormationType formationType)
        {
            currentFormation = formationType;
            SkyboundDebug.Log($"Formation changed to {currentFormation}.", this);
        }

        public void MoveUnits(IReadOnlyList<SelectableUnit> units, Vector3 destination)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsPaused)
            {
                SkyboundDebug.Log("Move command ignored because game is paused.", this);
                return;
            }

            if (units == null || units.Count == 0)
            {
                SkyboundDebug.Warning("MoveUnits called with no selected units.", this);
                return;
            }

            for (int i = 0; i < units.Count; i++)
            {
                SelectableUnit selectedUnit = units[i];

                if (selectedUnit == null)
                {
                    SkyboundDebug.Warning($"Selected unit at index {i} was null. Skipping movement.", this);
                    continue;
                }

                Vector3 offset = FormationUtility.GetOffset(
                    i,
                    currentFormation,
                    formationColumns,
                    formationSpacing
                );

                Vector3 finalDestination = destination + offset;
                Vector3 markerDestination = finalDestination;
                markerDestination.y += markerGroundOffset;

                CombatAttackController attackController =
                    selectedUnit.GetComponent<CombatAttackController>();

                if (attackController != null)
                    attackController.ClearTarget();

                selectedUnit.MoveTo(finalDestination);
                SpawnMoveMarker(selectedUnit, markerDestination);
            }

            SkyboundDebug.Log(
                $"Issued move command to {units.Count} unit(s) using {currentFormation} formation.",
                this
            );
        }

        private void SpawnMoveMarker(SelectableUnit unit, Vector3 destination)
        {
            if (moveMarkerPrefab == null)
                return;

            if (unit == null)
            {
                SkyboundDebug.Warning("Tried to spawn move marker for null unit.", this);
                return;
            }

            UnitMoveMarkerController markerController =
                unit.GetComponent<UnitMoveMarkerController>();

            if (markerController == null)
            {
                markerController = unit.gameObject.AddComponent<UnitMoveMarkerController>();
                SkyboundDebug.Log($"{unit.name} was missing UnitMoveMarkerController. Added one at runtime.", unit);
            }

            MoveMarker marker = Instantiate(
                moveMarkerPrefab,
                destination,
                Quaternion.identity
            );

            marker.Initialize(unit.transform, destination, markerController);
            markerController.SetMarker(marker);
        }

        private void ValidateSettings()
        {
            if (formationSpacing <= 0f)
            {
                SkyboundDebug.Warning("MoveCommandSystem formationSpacing was invalid. Resetting to 1.5.", this);
                formationSpacing = 1.5f;
            }

            if (formationColumns <= 0)
            {
                SkyboundDebug.Warning("MoveCommandSystem formationColumns was invalid. Resetting to 1.", this);
                formationColumns = 1;
            }

            if (moveMarkerPrefab == null)
            {
                SkyboundDebug.Warning(
                    "MoveCommandSystem has no moveMarkerPrefab assigned. Movement will still work, but markers will not spawn.",
                    this
                );
            }
        }
    }
}