using System.Collections.Generic;
using Skybound.Characters;
using Skybound.Combat;
using Skybound.Core.Diagnostics;
using UnityEngine;

namespace Skybound.InputSystem
{
    public class AttackCommandSystem : MonoBehaviour
    {
        public void AttackTarget(
            IReadOnlyList<SelectableUnit> units,
            CharacterStats target)
        {
            if (target == null)
            {
                SkyboundDebug.Warning("Tried to issue attack command against null target.", this);
                return;
            }

            if (units == null || units.Count == 0)
            {
                SkyboundDebug.Warning("AttackTarget called with no selected units.", this);
                return;
            }

            foreach (SelectableUnit unit in units)
            {
                if (unit == null)
                    continue;

                UnitMoveMarkerController markerController =
                    unit.GetComponent<UnitMoveMarkerController>();

                if (markerController != null)
                    markerController.ClearMarker();

                CombatAttackController attackController =
                    unit.GetComponent<CombatAttackController>();

                if (attackController == null)
                {
                    SkyboundDebug.Warning($"{unit.name} has no CombatAttackController. Cannot attack.", unit);
                    continue;
                }

                attackController.SetAttackTarget(target);
            }

            SkyboundDebug.Log(
                $"Issued attack command against {target.name} to {units.Count} unit(s).",
                this
            );
        }
    }
}