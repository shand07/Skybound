using UnityEngine;

namespace Skybound.Systems.FogOfWar
{
    public class FogVisibilityTarget : MonoBehaviour
    {
        [SerializeField] private bool visibleOnlyInCurrentVision = true;
        [SerializeField] private Renderer[] renderersToControl;

        private void Awake()
        {
            if (renderersToControl == null || renderersToControl.Length == 0)
                renderersToControl = GetComponentsInChildren<Renderer>();
        }

        private void Update()
        {
            if (FogOfWarManager.Instance == null)
                return;

            FogState fogState = FogOfWarManager.Instance.GetFogStateAtWorldPosition(transform.position);

            bool shouldBeVisible = visibleOnlyInCurrentVision
                ? fogState == FogState.Visible
                : fogState != FogState.Unexplored;

            SetVisible(shouldBeVisible);
        }

        private void SetVisible(bool visible)
        {
            foreach (Renderer targetRenderer in renderersToControl)
            {
                if (targetRenderer != null)
                    targetRenderer.enabled = visible;
            }
        }
    }
}