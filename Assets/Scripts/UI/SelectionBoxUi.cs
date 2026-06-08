using UnityEngine;

namespace Skybound.UI
{
    public class SelectionBoxUI : MonoBehaviour
    {
        [SerializeField] private RectTransform selectionBox;

        private Vector2 startPosition;

        private void Awake()
        {
            if (selectionBox != null)
                selectionBox.gameObject.SetActive(false);
        }

        public void BeginSelection(Vector2 screenPosition)
        {
            if (selectionBox == null)
                return;

            startPosition = screenPosition;
            selectionBox.gameObject.SetActive(true);
            UpdateSelection(screenPosition);
        }

        public void UpdateSelection(Vector2 currentPosition)
        {
            if (selectionBox == null)
                return;

            Vector2 lowerLeft = Vector2.Min(startPosition, currentPosition);
            Vector2 upperRight = Vector2.Max(startPosition, currentPosition);
            Vector2 size = upperRight - lowerLeft;

            selectionBox.position = lowerLeft + size * 0.5f;
            selectionBox.sizeDelta = size;
        }

        public void EndSelection()
        {
            if (selectionBox != null)
                selectionBox.gameObject.SetActive(false);
        }
    }
}