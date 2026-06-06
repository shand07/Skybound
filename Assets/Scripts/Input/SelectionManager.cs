using System.Collections.Generic;
using Skybound.Characters;
using Skybound.Core.Diagnostics;
using Skybound.UI;
using UnityEngine;

namespace Skybound.InputSystem
{
    public class SelectionManager : MonoBehaviour
    {
        [Header("Layers")]
        [SerializeField] private LayerMask selectableLayer;
        [SerializeField] private LayerMask groundLayer;

        [Header("References")]
        [SerializeField] private Camera mainCamera;

        [Header("Systems")]
        [SerializeField] private MoveCommandSystem moveCommandSystem;
        [SerializeField] private AttackCommandSystem attackCommandSystem;

        [Header("UI")]
        [SerializeField] private SelectionBoxUI selectionBoxUI;

        [Header("Drag Select")]
        [SerializeField] private float dragThreshold = 10f;

        private readonly List<SelectableUnit> selectedUnits = new();

        private Vector2 dragStartPosition;
        private bool isDragging;

        private void Awake()
        {
            ResolveReferences();
            ValidateSettings();
        }

        private void Update()
        {
            HandleInput();
        }

        private void ResolveReferences()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera == null)
            {
                SkyboundDebug.MissingReference(
                    this,
                    nameof(mainCamera),
                    "Assign a camera in the inspector or tag the main camera as MainCamera."
                );
            }

            if (moveCommandSystem == null)
                moveCommandSystem = GetComponent<MoveCommandSystem>();

            if (moveCommandSystem == null)
            {
                SkyboundDebug.MissingReference(
                    this,
                    nameof(moveCommandSystem),
                    "Add MoveCommandSystem to the same GameObject as SelectionManager, or assign it manually."
                );
            }

            if (attackCommandSystem == null)
                attackCommandSystem = GetComponent<AttackCommandSystem>();

            if (attackCommandSystem == null)
            {
                SkyboundDebug.MissingReference(
                    this,
                    nameof(attackCommandSystem),
                    "Add AttackCommandSystem to the same GameObject as SelectionManager, or assign it manually."
                );
            }
        }

        private void ValidateSettings()
        {
            if (selectableLayer == 0)
                SkyboundDebug.Warning("SelectionManager selectableLayer is empty. Unit selection may not work.", this);

            if (groundLayer == 0)
                SkyboundDebug.Warning("SelectionManager groundLayer is empty. Movement commands may not work.", this);

            if (selectionBoxUI == null)
            {
                SkyboundDebug.Warning(
                    "SelectionManager has no SelectionBoxUI assigned. Drag selection still works, but the box will not display.",
                    this
                );
            }

            if (dragThreshold < 0f)
            {
                SkyboundDebug.Warning("SelectionManager dragThreshold was negative. Resetting to 10.", this);
                dragThreshold = 10f;
            }
        }

        private void HandleInput()
        {
            if (mainCamera == null)
                return;

            if (Input.GetMouseButtonDown(1))
            {
                ClearSelection();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                dragStartPosition = Input.mousePosition;
                isDragging = true;

                if (selectionBoxUI != null)
                    selectionBoxUI.BeginSelection(dragStartPosition);
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                if (selectionBoxUI != null)
                    selectionBoxUI.UpdateSelection(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 dragEndPosition = Input.mousePosition;
                isDragging = false;

                if (selectionBoxUI != null)
                    selectionBoxUI.EndSelection();

                float dragDistance = Vector2.Distance(dragStartPosition, dragEndPosition);

                if (dragDistance >= dragThreshold)
                {
                    BoxSelect(dragStartPosition, dragEndPosition);
                    return;
                }

                HandleSingleLeftClick();
            }
        }

        private void HandleSingleLeftClick()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit unitHit, 500f, selectableLayer))
            {
                SelectableUnit unit = unitHit.collider.GetComponentInParent<SelectableUnit>();
                UnitIdentity identity = unitHit.collider.GetComponentInParent<UnitIdentity>();
                CharacterStats targetStats = unitHit.collider.GetComponentInParent<CharacterStats>();

                if (unit != null && unit.CanBeSelectedByPlayer())
                {
                    ClearSelection();
                    SelectUnit(unit);
                    return;
                }

                if (identity != null &&
                    identity.IsHostileToPlayer() &&
                    targetStats != null &&
                    selectedUnits.Count > 0)
                {
                    IssueAttackCommand(targetStats);
                    return;
                }
            }

            if (Physics.Raycast(ray, out RaycastHit groundHit, 500f, groundLayer))
            {
                if (selectedUnits.Count > 0)
                    IssueMoveCommand(groundHit.point);
            }
        }

        private void IssueMoveCommand(Vector3 destination)
        {
            if (moveCommandSystem == null)
            {
                SkyboundDebug.Warning(
                    "Cannot issue move command because MoveCommandSystem is missing.",
                    this
                );

                return;
            }

            moveCommandSystem.MoveUnits(selectedUnits, destination);
        }

        private void IssueAttackCommand(CharacterStats target)
        {
            if (attackCommandSystem == null)
            {
                SkyboundDebug.Warning(
                    "Cannot issue attack command because AttackCommandSystem is missing.",
                    this
                );

                return;
            }

            attackCommandSystem.AttackTarget(selectedUnits, target);
        }

        private void BoxSelect(Vector2 start, Vector2 end)
        {
            Rect selectionRect = GetScreenRect(start, end);
            SelectableUnit[] allUnits = FindObjectsByType<SelectableUnit>(FindObjectsSortMode.None);

            List<SelectableUnit> unitsInBox = new();

            foreach (SelectableUnit unit in allUnits)
            {
                if (unit == null || !unit.CanBeSelectedByPlayer())
                    continue;

                Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);

                if (screenPos.z > 0f && selectionRect.Contains(screenPos))
                    unitsInBox.Add(unit);
            }

            if (unitsInBox.Count == 0)
            {
                SkyboundDebug.Log("Box select found no selectable units.", this);
                return;
            }

            ClearSelection();

            foreach (SelectableUnit unit in unitsInBox)
                SelectUnit(unit);

            SkyboundDebug.Log($"Box selected {unitsInBox.Count} unit(s).", this);
        }

        private void SelectUnit(SelectableUnit unit)
        {
            if (unit == null)
            {
                SkyboundDebug.Warning("Tried to select a null unit.", this);
                return;
            }

            if (!unit.CanBeSelectedByPlayer())
            {
                SkyboundDebug.Warning($"{unit.name} cannot be selected by player.", unit);
                return;
            }

            if (selectedUnits.Contains(unit))
                return;

            selectedUnits.Add(unit);
            unit.Select();
        }

        private void ClearSelection()
        {
            foreach (SelectableUnit unit in selectedUnits)
            {
                if (unit != null)
                    unit.Deselect();
            }

            if (selectedUnits.Count > 0)
                SkyboundDebug.Log($"Cleared selection of {selectedUnits.Count} unit(s).", this);

            selectedUnits.Clear();
        }

        private Rect GetScreenRect(Vector2 start, Vector2 end)
        {
            Vector2 lowerLeft = Vector2.Min(start, end);
            Vector2 upperRight = Vector2.Max(start, end);

            return Rect.MinMaxRect(
                lowerLeft.x,
                lowerLeft.y,
                upperRight.x,
                upperRight.y
            );
        }
    }
}