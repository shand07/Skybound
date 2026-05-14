using UnityEngine;

namespace Skybound.UI
{
    public class SelectionBoxUI : MonoBehaviour
    {
        [SerializeField] private RectTransform selectionBox;

        private Vector2 startPosition;

        private void Awake()
        {
            selectionBox.gameObject.SetActive(false);
        }

        public void BeginSelection(Vector2 screenPosition)
        {
            startPosition = screenPosition;
            selectionBox.gameObject.SetActive(true);
            UpdateSelection(screenPosition);
        }

        public void UpdateSelection(Vector2 currentPosition)
        {
            Vector2 lowerLeft = Vector2.Min(startPosition, currentPosition);
            Vector2 upperRight = Vector2.Max(startPosition, currentPosition);

            selectionBox.position = lowerLeft;
            selectionBox.sizeDelta = upperRight - lowerLeft;
        }

        public void EndSelection()
        {
            selectionBox.gameObject.SetActive(false);
        }
    }
}