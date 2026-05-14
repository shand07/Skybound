using System.Collections.Generic;
using Skybound.Characters;
using Skybound.Core;
using UnityEngine;
using Skybound.Combat;
using Skybound.UI;

namespace Skybound.InputSystem
{
    public class SelectionManager : MonoBehaviour
    {
        [Header("Layers")]
        [SerializeField] private LayerMask selectableLayer;
        [SerializeField] private LayerMask groundLayer;

        [Header("References")]
        [SerializeField] private Camera mainCamera;

        [Header("Drag Select")]
        [SerializeField] private float dragThreshold = 10f;

        [Header("Formation")]
        [SerializeField] private float formationSpacing = 1.5f;
        [SerializeField] private int formationColumns = 3;
        
        [Header("Move Markers")]
        [SerializeField] private MoveMarker moveMarkerPrefab;
        [SerializeField] private float markerGroundOffset = 0.05f;

        private readonly List<SelectableUnit> selectedUnits = new();

        private Vector2 dragStartPosition;
        private bool isDragging;

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleLeftClickInput();
        }

        private void HandleLeftClickInput()
        {
            
            if (Input.GetMouseButtonDown(1))
            {
                ClearSelection();
                return;
            }
            
            
            if (Input.GetMouseButtonDown(0))
            {
                dragStartPosition = Input.mousePosition;
                isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 dragEndPosition = Input.mousePosition;
                isDragging = false;

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

                // Select friendly controllable party units
                if (unit != null && unit.CanBeSelectedByPlayer())
                {
                    ClearSelection();
                    SelectUnit(unit);
                    return;
                }

                // Attack hostile targets
                if (identity != null &&
                    identity.IsHostileToPlayer() &&
                    targetStats != null &&
                    selectedUnits.Count > 0)
                {
                    IssueAttackCommand(targetStats);
                    return;
                }
            }

            // Move command on ground click
            if (Physics.Raycast(ray, out RaycastHit groundHit, 500f, groundLayer))
            {
                if (selectedUnits.Count > 0)
                {
                    MoveSelectedUnits(groundHit.point);
                }
            }
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
                {
                    unitsInBox.Add(unit);
                }
            }

            if (unitsInBox.Count == 0)
                return;

            ClearSelection();

            foreach (SelectableUnit unit in unitsInBox)
            {
                SelectUnit(unit);
            }
        }

        private void MoveSelectedUnits(Vector3 destination)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsPaused)
                return;

            for (int i = 0; i < selectedUnits.Count; i++)
            {
                Vector3 offset = GetFormationOffset(i);

                Vector3 finalDestination = destination + offset;
                finalDestination.y += markerGroundOffset;

                CombatAttackController attackController =
                    selectedUnits[i].GetComponent<CombatAttackController>();

                if (attackController != null)
                    attackController.ClearTarget();

                selectedUnits[i].MoveTo(destination + offset);

                SpawnMoveMarker(selectedUnits[i], finalDestination);
            }
        }
        
        private void SpawnMoveMarker(SelectableUnit unit, Vector3 destination)
        {
            if (moveMarkerPrefab == null || unit == null)
                return;

            UnitMoveMarkerController markerController =
                unit.GetComponent<UnitMoveMarkerController>();

            if (markerController == null)
                markerController = unit.gameObject.AddComponent<UnitMoveMarkerController>();

            MoveMarker marker = Instantiate(
                moveMarkerPrefab,
                destination,
                Quaternion.identity
            );

            marker.Initialize(unit.transform, destination, markerController);
            markerController.SetMarker(marker);
        }

        private Vector3 GetFormationOffset(int index)
        {
            if (formationColumns <= 0)
                formationColumns = 1;

            int row = index / formationColumns;
            int column = index % formationColumns;

            float centeredColumn = column - (formationColumns - 1) / 2f;

            return new Vector3(
                centeredColumn * formationSpacing,
                0f,
                row * formationSpacing
            );
        }

        private void SelectUnit(SelectableUnit unit)
        {
            if (unit == null)
                return;

            if (!unit.CanBeSelectedByPlayer())
                return;

            if (selectedUnits.Contains(unit))
                return;

            selectedUnits.Add(unit);
            unit.Select();

            Debug.Log("Selected: " + unit.name);
        }

        private void ClearSelection()
        {
            foreach (SelectableUnit unit in selectedUnits)
            {
                if (unit != null)
                    unit.Deselect();
            }

            selectedUnits.Clear();
        }

        private Rect GetScreenRect(Vector2 start, Vector2 end)
        {
            Vector2 bottomLeft = Vector2.Min(start, end);
            Vector2 topRight = Vector2.Max(start, end);

            return Rect.MinMaxRect(bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);
        }

        private void OnGUI()
        {
            if (!isDragging)
                return;

            float dragDistance = Vector2.Distance(dragStartPosition, Input.mousePosition);

            if (dragDistance < dragThreshold)
                return;

            Rect rect = GetGUIRect(dragStartPosition, Input.mousePosition);

            GUI.color = new Color(0.2f, 0.6f, 1f, 0.25f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);

            GUI.color = new Color(0.2f, 0.6f, 1f, 1f);
            DrawRectBorder(rect, 2f);

            GUI.color = Color.white;
        }

        private Rect GetGUIRect(Vector2 start, Vector2 end)
        {
            start.y = Screen.height - start.y;
            end.y = Screen.height - end.y;

            Vector2 topLeft = Vector2.Min(start, end);
            Vector2 bottomRight = Vector2.Max(start, end);

            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }

        private void DrawRectBorder(Rect rect, float thickness)
        {
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);
        }
        
        private void IssueAttackCommand(CharacterStats target)
        {
            foreach (SelectableUnit selectedUnit in selectedUnits)
            {
                if (selectedUnit == null)
                    continue;

                CombatAttackController attackController = selectedUnit.GetComponent<CombatAttackController>();

                if (attackController != null)
                    attackController.SetAttackTarget(target);
            }
        }
    }
}