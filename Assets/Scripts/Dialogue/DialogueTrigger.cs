using Skybound.Core.Diagnostics;
using Skybound.Data;
using UnityEngine;

namespace Skybound.Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private DialogueData dialogueData;

        [Header("Trigger Rules")]
        [SerializeField] private bool triggerOnce = true;

        private bool hasTriggered;

        public void TriggerDialogue()
        {
            if (triggerOnce && hasTriggered)
                return;

            if (dialogueData == null)
            {
                SkyboundDebug.MissingReference(this, nameof(dialogueData));
                return;
            }

            if (DialogueRunner.Instance == null)
            {
                SkyboundDebug.ServiceUnavailable(this, nameof(DialogueRunner));
                return;
            }

            DialogueRunner.Instance.StartDialogue(dialogueData);
            hasTriggered = true;
        }
    }
}