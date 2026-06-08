using Skybound.Core.Diagnostics;
using Skybound.Core.Services;
using UnityEngine;

namespace Skybound.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool IsPaused { get; private set; }
        public bool IsPauseLocked { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                SkyboundDebug.Warning("Duplicate GameManager detected. Destroying duplicate.", this);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            SkyboundServiceRegistry.Register(this);

            ForceResume();

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
            if (IsPauseLocked)
            {
                SkyboundDebug.Log("Pause toggle blocked because pause is currently locked.", this);
                return;
            }

            SetPaused(!IsPaused);
        }

        public void SetPauseLocked(bool locked)
        {
            IsPauseLocked = locked;

            if (locked)
                SetPaused(true);
            else
                SetPaused(false);

            SkyboundDebug.Log(locked ? "Game pause locked." : "Game pause unlocked.", this);
        }

        private void SetPaused(bool paused)
        {
            IsPaused = paused;
            Time.timeScale = IsPaused ? 0f : 1f;

            SkyboundDebug.Log($"Pause state changed. IsPaused = {IsPaused}", this);
        }

        private void ForceResume()
        {
            IsPauseLocked = false;
            IsPaused = false;
            Time.timeScale = 1f;
        }
    }
}