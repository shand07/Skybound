using Skybound.Core.Diagnostics;
using Skybound.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skybound.Dialogue
{
    public class GameDialogueBoxView : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject root;

        [Header("Speaker")]
        [SerializeField] private TMP_Text speakerNameText;
        [SerializeField] private Image speakerPortraitImage;

        [Header("Dialogue")]
        [SerializeField] private TMP_Text dialogueText;

        [Header("Action Button")]
        [SerializeField] private Button actionButton;
        [SerializeField] private TMP_Text actionButtonText;

        [Header("Choices")]
        [SerializeField] private Transform choiceContainer;
        [SerializeField] private DialogueChoiceButton choiceButtonPrefab;

        private bool currentNodeEndsDialogue;

        private void Awake()
        {
            ValidateReferences();
            Hide();
        }

        private void OnEnable()
        {
            if (actionButton != null)
                actionButton.onClick.AddListener(HandleActionButtonClicked);
        }

        private void OnDisable()
        {
            if (actionButton != null)
                actionButton.onClick.RemoveListener(HandleActionButtonClicked);
        }

        public void Show(DialogueNodeData node)
        {
            if (node == null)
            {
                SkyboundDebug.Warning("GameDialogueBoxView tried to show null node.", this);
                Hide();
                return;
            }

            currentNodeEndsDialogue = node.EndsDialogue;

            if (root != null)
                root.SetActive(true);

            if (speakerNameText != null)
                speakerNameText.text = node.SpeakerName;

            if (dialogueText != null)
                dialogueText.text = node.Text;

            if (speakerPortraitImage != null)
            {
                speakerPortraitImage.sprite = node.SpeakerPortrait;
                speakerPortraitImage.enabled = node.SpeakerPortrait != null;
            }

            RefreshChoices(node);

            bool hasChoices = node.HasChoices;

            if (actionButton != null)
                actionButton.gameObject.SetActive(!hasChoices);

            if (actionButtonText != null)
                actionButtonText.text = node.EndsDialogue ? "End Dialogue" : "Continue";
        }

        public void Hide()
        {
            ClearChoices();

            if (root != null)
                root.SetActive(false);
        }

        private void RefreshChoices(DialogueNodeData node)
        {
            ClearChoices();

            if (!node.HasChoices)
            {
                if (choiceContainer != null)
                    choiceContainer.gameObject.SetActive(false);

                return;
            }

            if (choiceContainer == null)
            {
                SkyboundDebug.MissingReference(this, nameof(choiceContainer));
                return;
            }

            if (choiceButtonPrefab == null)
            {
                SkyboundDebug.Warning(
                    "Choice node found, but no choiceButtonPrefab is assigned. Choices cannot be displayed.",
                    this
                );

                return;
            }

            choiceContainer.gameObject.SetActive(true);

            DialogueChoiceData[] choices = node.Choices;

            for (int i = 0; i < choices.Length; i++)
            {
                DialogueChoiceData choice = choices[i];

                if (choice == null)
                {
                    SkyboundDebug.Warning($"GameDialogueBoxView found null choice at index {i}.", this);
                    continue;
                }

                DialogueChoiceButton button = Instantiate(
                    choiceButtonPrefab,
                    choiceContainer
                );

                button.Initialize(choice.ChoiceText, i);
            }
        }

        private void ClearChoices()
        {
            if (choiceContainer == null)
                return;

            for (int i = choiceContainer.childCount - 1; i >= 0; i--)
                Destroy(choiceContainer.GetChild(i).gameObject);

            choiceContainer.gameObject.SetActive(false);
        }

        private void HandleActionButtonClicked()
        {
            if (currentNodeEndsDialogue)
            {
                DialogueRunner.Instance?.EndDialogue();
                return;
            }

            DialogueRunner.Instance?.Continue();
        }

        private void ValidateReferences()
        {
            if (root == null)
                SkyboundDebug.MissingReference(this, nameof(root));

            if (speakerNameText == null)
                SkyboundDebug.MissingReference(this, nameof(speakerNameText));

            if (dialogueText == null)
                SkyboundDebug.MissingReference(this, nameof(dialogueText));

            if (actionButton == null)
                SkyboundDebug.MissingReference(this, nameof(actionButton));

            if (actionButtonText == null)
                SkyboundDebug.MissingReference(this, nameof(actionButtonText));

            if (choiceContainer == null)
                SkyboundDebug.MissingReference(this, nameof(choiceContainer));

            if (choiceButtonPrefab == null)
            {
                SkyboundDebug.Warning(
                    "No choiceButtonPrefab assigned. Choice dialogue will be unavailable until one is assigned.",
                    this
                );
            }
        }
    }
}