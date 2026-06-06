using Skybound.Core.Diagnostics;
using Skybound.Core.Services;
using UnityEngine;

namespace Skybound.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                SkyboundDebug.Warning("Duplicate GameManager found. Destroying duplicate.", this);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            SkyboundServiceRegistry.Register(this);

            Time.timeScale = 1f;
            IsPaused = false;

            SkyboundDebug.Log("GameManager initialized.", this);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                SkyboundServiceRegistry.Unregister<GameManager>();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                TogglePause();
        }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
            Time.timeScale = IsPaused ? 0f : 1f;

            SkyboundDebug.Log(IsPaused ? "Game paused." : "Game unpaused.", this);
        }
    }
}