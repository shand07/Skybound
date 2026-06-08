using Skybound.Core.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skybound.Dialogue
{
    public class DialogueChoiceButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text choiceText;
        [SerializeField] private Button button;

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
                choiceText.text = text;
        }

        private void HandleClicked()
        {
            DialogueRunner.Instance?.SelectChoice(choiceIndex);
        }
    }
}