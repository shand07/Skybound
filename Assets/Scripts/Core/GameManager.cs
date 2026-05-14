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
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // Reset game state on startup
            Time.timeScale = 1f;
            IsPaused = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TogglePause();
            }
        }

        private void TogglePause()
        {
            IsPaused = !IsPaused;
            Time.timeScale = IsPaused ? 0f : 1f;
        }
    }
}