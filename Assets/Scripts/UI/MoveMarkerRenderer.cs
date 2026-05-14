using UnityEngine;

namespace Skybound.UI
{
    [RequireComponent(typeof(LineRenderer))]
    public class MoveMarkerRenderer : MonoBehaviour
    {
        [Header("Marker Shape")]
        [SerializeField] private float distanceFromCenter = 0.35f;
        [SerializeField] private float markerLength = 0.35f;
        [SerializeField] private float markerWidth = 0.22f;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();

            lineRenderer.useWorldSpace = false;
            lineRenderer.loop = false;
            lineRenderer.positionCount = 16;

            DrawMarker();
        }

        private void DrawMarker()
        {
            int index = 0;

            DrawWedge(ref index, Vector3.forward);
            DrawWedge(ref index, Vector3.right);
            DrawWedge(ref index, Vector3.back);
            DrawWedge(ref index, Vector3.left);
        }

        private void DrawWedge(ref int index, Vector3 direction)
        {
            Vector3 perpendicular = new Vector3(-direction.z, 0f, direction.x);

            Vector3 innerPoint = direction * distanceFromCenter;
            Vector3 outerPoint = direction * (distanceFromCenter + markerLength);

            Vector3 left = innerPoint + perpendicular * markerWidth;
            Vector3 right = innerPoint - perpendicular * markerWidth;

            lineRenderer.SetPosition(index++, outerPoint);
            lineRenderer.SetPosition(index++, left);
            lineRenderer.SetPosition(index++, right);
            lineRenderer.SetPosition(index++, outerPoint);
        }
    }
}