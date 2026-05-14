using UnityEngine;

namespace Skybound.Systems.FogOfWar
{
    public class FogTile : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] private Material unexploredMaterial;
        [SerializeField] private Material exploredMaterial;
        [SerializeField] private Material visibleMaterial;

        private Renderer tileRenderer;

        public FogState State { get; private set; } = FogState.Unexplored;

        private void Awake()
        {
            tileRenderer = GetComponent<Renderer>();
            SetState(FogState.Unexplored);
        }

        public void SetState(FogState newState)
        {
            State = newState;

            if (tileRenderer == null)
                tileRenderer = GetComponent<Renderer>();

            switch (State)
            {
                case FogState.Visible:
                    tileRenderer.material = visibleMaterial;
                    break;

                case FogState.Explored:
                    tileRenderer.material = exploredMaterial;
                    break;

                case FogState.Unexplored:
                    tileRenderer.material = unexploredMaterial;
                    break;
            }
        }
    }
}