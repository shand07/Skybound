using Skybound.Core.Diagnostics;
using UnityEngine;

namespace Skybound.Dialogue
{
    [RequireComponent(typeof(DialogueTrigger))]
    public class DialogueInteractable : MonoBehaviour
    {
        private DialogueTrigger dialogueTrigger;

        private void Awake()
        {
            dialogueTrigger = GetComponent<DialogueTrigger>();

            if (dialogueTrigger == null)
                SkyboundDebug.MissingReference(this, nameof(DialogueTrigger));
        }

        public void Interact()
        {
            if (dialogueTrigger == null)
            {
                SkyboundDebug.MissingReference(
                    this,
                    nameof(DialogueTrigger),
                    "Cannot start dialogue interaction."
                );

                return;
            }

            dialogueTrigger.TriggerDialogue();
        }
    }
}