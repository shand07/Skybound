using System;
using Skybound.Core;
using Skybound.Core.Diagnostics;
using Skybound.Data;
using UnityEngine;

namespace Skybound.Dialogue
{
    public class DialogueRunner : MonoBehaviour
    {
        public static DialogueRunner Instance { get; private set; }

        public event Action<DialogueNodeData> OnNodeChanged;
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;

        private DialogueData currentDialogue;
        private DialogueNodeData currentNode;
        private bool isRunning;

        public DialogueData CurrentDialogue => currentDialogue;
        public DialogueNodeData CurrentNode => currentNode;
        public bool IsRunning => isRunning;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                SkyboundDebug.Warning("Duplicate DialogueRunner found. Destroying duplicate.", this);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            SkyboundDebug.Log("DialogueRunner initialized.", this);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void StartDialogue(DialogueData dialogue)
        {
            if (dialogue == null)
            {
                SkyboundDebug.Warning("Tried to start null dialogue.", this);
                return;
            }

            if (!dialogue.IsValid(out string errorMessage))
            {
                SkyboundDebug.Error(
                    $"Cannot start invalid dialogue '{dialogue.name}': {errorMessage}",
                    this
                );

                return;
            }

            DialogueNodeData startNode = dialogue.GetStartNode();

            if (startNode == null)
            {
                SkyboundDebug.Error(
                    $"Dialogue '{dialogue.name}' has no valid start node '{dialogue.StartNodeId}'.",
                    this
                );

                return;
            }

            GameManager.Instance?.SetPauseLocked(true);

            currentDialogue = dialogue;
            currentNode = startNode;
            isRunning = true;

            SkyboundDebug.Log($"Started dialogue '{dialogue.DialogueName}'.", this);

            OnDialogueStarted?.Invoke();
            OnNodeChanged?.Invoke(currentNode);
        }

        public void Continue()
        {
            if (!isRunning)
            {
                SkyboundDebug.Warning("Continue called while no dialogue is running.", this);
                return;
            }

            if (currentNode == null)
            {
                SkyboundDebug.Error("DialogueRunner has no current node while dialogue is running.", this);
                EndDialogue();
                return;
            }

            if (currentNode.EndsDialogue)
            {
                EndDialogue();
                return;
            }

            if (currentNode.HasChoices)
            {
                SkyboundDebug.Warning(
                    $"Continue called on choice node '{currentNode.NodeId}'. Player must select a choice.",
                    this
                );

                return;
            }

            GoToNode(currentNode.NextNodeId);
        }

        public void SelectChoice(int choiceIndex)
        {
            if (!isRunning)
            {
                SkyboundDebug.Warning("SelectChoice called while no dialogue is running.", this);
                return;
            }

            if (currentNode == null)
            {
                SkyboundDebug.Error("DialogueRunner has no current node while selecting choice.", this);
                EndDialogue();
                return;
            }

            if (!currentNode.HasChoices)
            {
                SkyboundDebug.Warning(
                    $"SelectChoice called on node '{currentNode.NodeId}', but it has no choices.",
                    this
                );

                return;
            }

            DialogueChoiceData[] choices = currentNode.Choices;

            if (choiceIndex < 0 || choiceIndex >= choices.Length)
            {
                SkyboundDebug.Warning(
                    $"Invalid choice index {choiceIndex} on node '{currentNode.NodeId}'.",
                    this
                );

                return;
            }

            DialogueChoiceData choice = choices[choiceIndex];

            SkyboundDebug.Log(
                $"Selected choice '{choice.ChoiceText}' on node '{currentNode.NodeId}'.",
                this
            );

            GoToNode(choice.NextNodeId);
        }

        public void EndDialogue()
        {
            if (!isRunning)
                return;

            string dialogueName = currentDialogue != null
                ? currentDialogue.DialogueName
                : "Unknown Dialogue";

            currentDialogue = null;
            currentNode = null;
            isRunning = false;

            GameManager.Instance?.SetPauseLocked(false);

            SkyboundDebug.Log($"Ended dialogue '{dialogueName}'.", this);

            OnDialogueEnded?.Invoke();
        }

        private void GoToNode(string nodeId)
        {
            if (currentDialogue == null)
            {
                SkyboundDebug.Error("Cannot go to node because currentDialogue is null.", this);
                EndDialogue();
                return;
            }

            DialogueNodeData nextNode = currentDialogue.GetNodeById(nodeId);

            if (nextNode == null)
            {
                SkyboundDebug.Error(
                    $"Dialogue '{currentDialogue.DialogueName}' could not find node '{nodeId}'. Ending dialogue.",
                    this
                );

                EndDialogue();
                return;
            }

            currentNode = nextNode;

            SkyboundDebug.Log(
                $"Dialogue advanced to node '{currentNode.NodeId}' using mode {currentNode.PresentationMode}.",
                this
            );

            OnNodeChanged?.Invoke(currentNode);
        }
    }
}