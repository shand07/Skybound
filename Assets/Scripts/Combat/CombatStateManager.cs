using System;
using System.Collections.Generic;
using Skybound.Core.Diagnostics;
using Skybound.Core.Services;
using UnityEngine;

namespace Skybound.Combat
{
    public class CombatStateManager : MonoBehaviour
    {
        public static CombatStateManager Instance { get; private set; }

        [Header("Combat Exit")]
        [SerializeField] private float exitCombatGracePeriod = 3f;

        public bool IsInCombat { get; private set; }

        public event Action<bool> OnCombatStateChanged;

        private readonly HashSet<GameObject> activeEnemies = new();

        private float exitCombatTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                SkyboundDebug.Warning("Duplicate CombatStateManager found. Destroying duplicate.", this);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            SkyboundServiceRegistry.Register(this);

            IsInCombat = false;
            exitCombatTimer = 0f;

            SkyboundDebug.Log("CombatStateManager initialized.", this);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                SkyboundServiceRegistry.Unregister<CombatStateManager>();
            }
        }

        private void Update()
        {
            RemoveDeadOrInactiveEnemies();

            if (Input.GetKeyDown(KeyCode.C))
                EnterCombat();

            if (Input.GetKeyDown(KeyCode.V))
                ForceExitCombat();

            if (!IsInCombat)
                return;

            if (activeEnemies.Count > 0)
            {
                exitCombatTimer = 0f;
                return;
            }

            exitCombatTimer += Time.deltaTime;

            if (exitCombatTimer >= exitCombatGracePeriod)
                ExitCombat();
        }

        public void EnterCombat()
        {
            if (IsInCombat)
                return;

            IsInCombat = true;
            exitCombatTimer = 0f;

            OnCombatStateChanged?.Invoke(IsInCombat);

            SkyboundDebug.Log("Entered combat.", this);
        }

        public void RegisterEnemy(GameObject enemy)
        {
            if (enemy == null)
            {
                SkyboundDebug.Warning("Tried to register a null enemy.", this);
                return;
            }

            bool added = activeEnemies.Add(enemy);

            if (added)
                SkyboundDebug.Log($"Registered enemy: {enemy.name}. Active enemies: {activeEnemies.Count}", enemy);

            EnterCombat();
        }

        public void UnregisterEnemy(GameObject enemy)
        {
            if (enemy == null)
            {
                SkyboundDebug.Warning("Tried to unregister a null enemy.", this);
                return;
            }

            bool removed = activeEnemies.Remove(enemy);

            if (removed)
                SkyboundDebug.Log($"Unregistered enemy: {enemy.name}. Active enemies: {activeEnemies.Count}", enemy);
        }

        public void ForceExitCombat()
        {
            ExitCombat();
        }

        private void ExitCombat()
        {
            if (!IsInCombat)
                return;

            IsInCombat = false;
            exitCombatTimer = 0f;
            activeEnemies.Clear();

            OnCombatStateChanged?.Invoke(IsInCombat);

            SkyboundDebug.Log("Exited combat.", this);
        }

        private void RemoveDeadOrInactiveEnemies()
        {
            int removedCount = activeEnemies.RemoveWhere(enemy => enemy == null || !enemy.activeInHierarchy);

            if (removedCount > 0)
                SkyboundDebug.Log($"Removed {removedCount} dead/inactive enemies from combat tracking.", this);
        }
    }
}