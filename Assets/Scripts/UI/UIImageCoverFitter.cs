using UnityEngine;
using UnityEngine.UI;

namespace Skybound.UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class UIImageCoverFitter : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Image image;
        private Canvas rootCanvas;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
            rootCanvas = GetComponentInParent<Canvas>();
        }

        private void OnEnable()
        {
            Fit();
        }

        private void LateUpdate()
        {
            Fit();
        }

        private void Fit()
        {
            if (image == null || image.sprite == null || rootCanvas == null)
                return;

            RectTransform canvasRect = rootCanvas.transform as RectTransform;
            if (canvasRect == null)
                return;

            float screenWidth = canvasRect.rect.width;
            float screenHeight = canvasRect.rect.height;

            float imageWidth = image.sprite.rect.width;
            float imageHeight = image.sprite.rect.height;

            float screenAspect = screenWidth / screenHeight;
            float imageAspect = imageWidth / imageHeight;

            float targetWidth;
            float targetHeight;

            if (imageAspect > screenAspect)
            {
                targetHeight = screenHeight;
                targetWidth = targetHeight * imageAspect;
            }
            else
            {
                targetWidth = screenWidth;
                targetHeight = targetWidth / imageAspect;
            }

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
            rectTransform.localScale = Vector3.one;
        }
    }
}