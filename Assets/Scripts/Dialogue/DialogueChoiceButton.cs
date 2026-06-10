using Skybound.Core.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Skybound.Dialogue
{
    public class DialogueChoiceButton : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [Header("References")]
        [SerializeField] private TMP_Text choiceText;
        [SerializeField] private Button button;

        [Header("Colors")]
        [SerializeField] private Color normalColor =
            new Color32(140, 200, 255, 255);

        [SerializeField] private Color hoverColor =
            Color.white;

        private int choiceIndex;

        private void Awake()
        {
            if (choiceText == null)
                SkyboundDebug.MissingReference(this, nameof(choiceText));

            if (button == null)
                button = GetComponent<Button>();

            if (button == null)
                SkyboundDebug.MissingReference(this, nameof(button));
        }

        private void OnEnable()
        {
            if (button != null)
                button.onClick.AddListener(HandleClicked);

            ApplyNormalState();
        }

        private void OnDisable()
        {
            if (button != null)
                button.onClick.RemoveListener(HandleClicked);
        }

        public void Initialize(string text, int index)
        {
            choiceIndex = index;

            if (choiceText != null)
            {
                choiceText.text = text;
                choiceText.color = normalColor;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (choiceText != null)
                choiceText.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ApplyNormalState();
        }

        private void HandleClicked()
        {
            DialogueRunner.Instance?.SelectChoice(choiceIndex);
        }

        private void ApplyNormalState()
        {
            if (choiceText != null)
                choiceText.color = normalColor;
        }
    }
}