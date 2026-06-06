using UnityEngine;

namespace Skybound.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SelectionBoxUI : MonoBehaviour
    {
        private RectTransform selectionBox;

        private Vector2 startPosition;

        private void Awake()
        {
            selectionBox = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        public void BeginSelection(Vector2 screenPosition)
        {
            startPosition = screenPosition;

            gameObject.SetActive(true);

            UpdateSelection(screenPosition);
        }

        public void UpdateSelection(Vector2 currentPosition)
        {
            Vector2 lowerLeft = Vector2.Min(startPosition, currentPosition);
            Vector2 upperRight = Vector2.Max(startPosition, currentPosition);

            Vector2 size = upperRight - lowerLeft;

            selectionBox.position = lowerLeft + size * 0.5f;
            selectionBox.sizeDelta = size;
        }

        public void EndSelection()
        {
            gameObject.SetActive(false);
        }
    }
}