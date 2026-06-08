using UnityEngine;

namespace Skybound.Data
{
    [System.Serializable]
    public class DialogueChoiceData
    {
        [SerializeField] private string choiceText;
        [SerializeField] private string nextNodeId;

        public string ChoiceText => choiceText;
        public string NextNodeId => nextNodeId;

        public bool IsValid(out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(choiceText))
            {
                errorMessage = "Choice text is empty.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(nextNodeId))
            {
                errorMessage = $"Choice '{choiceText}' has no next node id.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}