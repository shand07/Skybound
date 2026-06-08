using Skybound.Core.Diagnostics;
using Skybound.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skybound.Dialogue
{
    public class VisualNovelDialogueView : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject root;

        [Header("Scene Image")]
        [SerializeField] private Image sceneImage;

        [Header("Speaker")]
        [SerializeField] private TMP_Text speakerNameText;
        [SerializeField] private Image speakerPortraitImage;

        [Header("Dialogue")]
        [SerializeField] private TMP_Text dialogueText;

        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button endDialogueButton;

        [Header("Choices")]
        [SerializeField] private Transform choiceContainer;
        [SerializeField] private DialogueChoiceButton choiceButtonPrefab;

        private void Awake()
        {
            ValidateReferences();
            Hide();
        }

        private void OnEnable()
        {
            if (continueButton != null)
                continueButton.onClick.AddListener(HandleContinueClicked);

            if (endDialogueButton != null)
                endDialogueButton.onClick.AddListener(HandleEndDialogueClicked);
        }

        private void OnDisable()
        {
            if (continueButton != null)
                continueButton.onClick.RemoveListener(HandleContinueClicked);

            if (endDialogueButton != null)
                endDialogueButton.onClick.RemoveListener(HandleEndDialogueClicked);
        }

        public void Show(DialogueNodeData node)
        {
            if (node == null)
            {
                SkyboundDebug.Warning("VisualNovelDialogueView tried to show null node.", this);
                Hide();
                return;
            }

            if (root != null)
                root.SetActive(true);

            if (sceneImage != null)
            {
                sceneImage.sprite = node.VisualNovelImage;
                sceneImage.enabled = node.VisualNovelImage != null;
            }

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

            if (continueButton != null)
                continueButton.gameObject.SetActive(!hasChoices && !node.EndsDialogue);

            if (endDialogueButton != null)
                endDialogueButton.gameObject.SetActive(node.EndsDialogue);
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
                SkyboundDebug.MissingReference(this, nameof(choiceButtonPrefab));
                return;
            }

            choiceContainer.gameObject.SetActive(true);

            DialogueChoiceData[] choices = node.Choices;

            for (int i = 0; i < choices.Length; i++)
            {
                DialogueChoiceData choice = choices[i];

                if (choice == null)
                {
                    SkyboundDebug.Warning($"VisualNovelDialogueView found null choice at index {i}.", this);
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

        private void HandleContinueClicked()
        {
            DialogueRunner.Instance?.Continue();
        }

        private void HandleEndDialogueClicked()
        {
            DialogueRunner.Instance?.EndDialogue();
        }

        private void ValidateReferences()
        {
            if (root == null)
                SkyboundDebug.MissingReference(this, nameof(root));

            if (sceneImage == null)
                SkyboundDebug.MissingReference(this, nameof(sceneImage));

            if (speakerNameText == null)
                SkyboundDebug.MissingReference(this, nameof(speakerNameText));

            if (dialogueText == null)
                SkyboundDebug.MissingReference(this, nameof(dialogueText));

            if (continueButton == null)
                SkyboundDebug.MissingReference(this, nameof(continueButton));

            if (endDialogueButton == null)
                SkyboundDebug.MissingReference(this, nameof(endDialogueButton));

            if (choiceContainer == null)
                SkyboundDebug.MissingReference(this, nameof(choiceContainer));

            if (choiceButtonPrefab == null)
                SkyboundDebug.MissingReference(this, nameof(choiceButtonPrefab));
        }
    }
}