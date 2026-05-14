using System;
using System.Collections.Generic;
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
                Destroy(gameObject);
                return;
            }

            Instance = this;
            IsInCombat = false;
        }

        private void Update()
        {
            RemoveDeadOrInactiveEnemies();

            if (!IsInCombat)
                return;

            if (activeEnemies.Count > 0)
            {
                exitCombatTimer = 0f;
                return;
            }

            exitCombatTimer += Time.deltaTime;

            if (exitCombatTimer >= exitCombatGracePeriod)
            {
                ExitCombat();
            }

            // temporary test keys
            if (Input.GetKeyDown(KeyCode.C))
                EnterCombat();

            if (Input.GetKeyDown(KeyCode.V))
                ForceExitCombat();
        }

        public void EnterCombat()
        {
            if (IsInCombat)
                return;

            IsInCombat = true;
            exitCombatTimer = 0f;

            OnCombatStateChanged?.Invoke(IsInCombat);

            Debug.Log("Entered Combat");
        }

        public void RegisterEnemy(GameObject enemy)
        {
            if (enemy == null)
                return;

            activeEnemies.Add(enemy);
            EnterCombat();
        }

        public void UnregisterEnemy(GameObject enemy)
        {
            if (enemy == null)
                return;

            activeEnemies.Remove(enemy);
        }

        private void ExitCombat()
        {
            if (!IsInCombat)
                return;

            IsInCombat = false;
            exitCombatTimer = 0f;
            activeEnemies.Clear();

            OnCombatStateChanged?.Invoke(IsInCombat);

            Debug.Log("Exited Combat");
        }

        public void ForceExitCombat()
        {
            ExitCombat();
        }

        private void RemoveDeadOrInactiveEnemies()
        {
            activeEnemies.RemoveWhere(enemy => enemy == null || !enemy.activeInHierarchy);
        }
    }
}