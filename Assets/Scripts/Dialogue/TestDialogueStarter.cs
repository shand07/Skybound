using Skybound.Data;
using UnityEngine;

namespace Skybound.Dialogue
{
    public class TestDialogueStarter : MonoBehaviour
    {
        [SerializeField] private DialogueData dialogueData;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                DialogueRunner.Instance?.StartDialogue(dialogueData);
            }
        }
    }
}