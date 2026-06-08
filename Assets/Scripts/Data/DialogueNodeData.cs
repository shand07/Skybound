using UnityEngine;

namespace Skybound.Data
{
    [System.Serializable]
    public class DialogueNodeData
    {
        [Header("Node")]
        [SerializeField] private string nodeId;
        [SerializeField] private string nextNodeId;
        [SerializeField] private bool endsDialogue;

        [Header("Presentation")]
        [SerializeField] private DialoguePresentationMode presentationMode =
            DialoguePresentationMode.GameDialogueBox;

        [Header("Speaker")]
        [SerializeField] private string speakerName;
        [SerializeField] private Sprite speakerPortrait;

        [Header("Text")]
        [TextArea(2, 8)]
        [SerializeField] private string text;

        [Header("Visual Novel")]
        [SerializeField] private Sprite visualNovelImage;

        [Header("Choices")]
        [SerializeField] private DialogueChoiceData[] choices;

        public string NodeId => nodeId;
        public string NextNodeId => nextNodeId;
        public bool EndsDialogue => endsDialogue;

        public DialoguePresentationMode PresentationMode => presentationMode;

        public string SpeakerName => speakerName;
        public Sprite SpeakerPortrait => speakerPortrait;

        public string Text => text;

        public Sprite VisualNovelImage => visualNovelImage;

        public DialogueChoiceData[] Choices => choices;
        public bool HasChoices => choices != null && choices.Length > 0;

        public bool IsValid(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                errorMessage = "Node id is empty.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(speakerName))
            {
                errorMessage = $"Node '{nodeId}' has no speaker name.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                errorMessage = $"Node '{nodeId}' has no dialogue text.";
                return false;
            }

            if (!endsDialogue && !HasChoices && string.IsNullOrWhiteSpace(nextNodeId))
            {
                errorMessage = $"Node '{nodeId}' does not end dialogue, has no choices, and has no next node id.";
                return false;
            }

            if (choices != null)
            {
                for (int i = 0; i < choices.Length; i++)
                {
                    DialogueChoiceData choice = choices[i];

                    if (choice == null)
                    {
                        errorMessage = $"Node '{nodeId}' has null choice at index {i}.";
                        return false;
                    }

                    if (!choice.IsValid(out string choiceError))
                    {
                        errorMessage = $"Node '{nodeId}' has invalid choice at index {i}: {choiceError}";
                        return false;
                    }
                }
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}