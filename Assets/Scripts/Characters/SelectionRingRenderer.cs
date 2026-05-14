using UnityEngine;

namespace Skybound.Characters
{
    [RequireComponent(typeof(LineRenderer))]
    public class SelectionRingRenderer : MonoBehaviour
    {
        [Header("Ring Settings")]
        [SerializeField] private float radius = 0.7f;
        [SerializeField] private int segments = 64;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();

            DrawRing();
        }

        private void DrawRing()
        {
            lineRenderer.positionCount = segments;

            float angleStep = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.Deg2Rad * angleStep * i;

                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                lineRenderer.SetPosition(i, new Vector3(x, 0f, z));
            }
        }
    }
}