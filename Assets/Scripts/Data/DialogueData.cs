using System.Collections.Generic;
using UnityEngine;

namespace Skybound.Data
{
    [CreateAssetMenu(fileName = "NewDialogueData", menuName = "Skybound/Dialogue/Dialogue Data")]
    public class DialogueData : SkyboundDataAsset
    {
        [Header("Identity")]
        [SerializeField] private string dialogueName;

        [Header("Start")]
        [SerializeField] private string startNodeId = "start";

        [Header("Nodes")]
        [SerializeField] private DialogueNodeData[] nodes;

        public string DialogueName => dialogueName;
        public string StartNodeId => startNodeId;
        public DialogueNodeData[] Nodes => nodes;

        public DialogueNodeData GetStartNode()
        {
            return GetNodeById(startNodeId);
        }

        public DialogueNodeData GetNodeById(string nodeId)
        {
            if (string.IsNullOrWhiteSpace(nodeId) || nodes == null)
                return null;

            foreach (DialogueNodeData node in nodes)
            {
                if (node == null)
                    continue;

                if (node.NodeId == nodeId)
                    return node;
            }

            return null;
        }

        protected override bool ValidateData(out string errorMessage)
        {
            if (!ValidateRequiredString(dialogueName, nameof(dialogueName), out errorMessage))
                return false;

            if (!ValidateRequiredString(startNodeId, nameof(startNodeId), out errorMessage))
                return false;

            if (nodes == null || nodes.Length == 0)
            {
                errorMessage = "Dialogue must contain at least one node.";
                return false;
            }

            HashSet<string> nodeIds = new();

            for (int i = 0; i < nodes.Length; i++)
            {
                DialogueNodeData node = nodes[i];

                if (node == null)
                {
                    errorMessage = $"Node at index {i} is null.";
                    return false;
                }

                if (!node.IsValid(out string nodeError))
                {
                    errorMessage = $"Node at index {i} is invalid: {nodeError}";
                    return false;
                }

                if (!nodeIds.Add(node.NodeId))
                {
                    errorMessage = $"Duplicate node id found: '{node.NodeId}'.";
                    return false;
                }
            }

            if (!nodeIds.Contains(startNodeId))
            {
                errorMessage = $"Start node id '{startNodeId}' does not match any node.";
                return false;
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                DialogueNodeData node = nodes[i];

                if (!node.EndsDialogue && !node.HasChoices)
                {
                    if (!nodeIds.Contains(node.NextNodeId))
                    {
                        errorMessage = $"Node '{node.NodeId}' points to missing next node id '{node.NextNodeId}'.";
                        return false;
                    }
                }

                if (node.HasChoices)
                {
                    foreach (DialogueChoiceData choice in node.Choices)
                    {
                        if (!nodeIds.Contains(choice.NextNodeId))
                        {
                            errorMessage = $"Node '{node.NodeId}' has choice '{choice.ChoiceText}' pointing to missing node id '{choice.NextNodeId}'.";
                            return false;
                        }
                    }
                }
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}