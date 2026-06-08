using Skybound.Core.Diagnostics;
using Skybound.Data;
using UnityEngine;

namespace Skybound.Dialogue
{
    public class DialogueViewController : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private GameDialogueBoxView gameDialogueBoxView;
        [SerializeField] private VisualNovelDialogueView visualNovelDialogueView;

        private DialogueRunner dialogueRunner;
        private bool isSubscribed;

        private void Awake()
        {
            ValidateReferences();
            HideAllViews();
        }

        private void OnEnable()
        {
            TrySubscribeToDialogueRunner();
        }

        private void Start()
        {
            TrySubscribeToDialogueRunner();
        }

        private void OnDisable()
        {
            UnsubscribeFromDialogueRunner();
        }

        private void TrySubscribeToDialogueRunner()
        {
            if (isSubscribed)
                return;

            if (dialogueRunner == null)
                dialogueRunner = DialogueRunner.Instance;

            if (dialogueRunner == null)
            {
                SkyboundDebug.Warning(
                    "DialogueViewController could not find DialogueRunner yet. Will retry on Start.",
                    this
                );

                return;
            }

            dialogueRunner.OnNodeChanged += HandleNodeChanged;
            dialogueRunner.OnDialogueEnded += HandleDialogueEnded;
            isSubscribed = true;

            SkyboundDebug.Log("DialogueViewController subscribed to DialogueRunner.", this);
            
            if (dialogueRunner.IsRunning && dialogueRunner.CurrentNode != null)
            {
                SkyboundDebug.Log("Dialogue was already running. Showing current dialogue node.", this);
                HandleNodeChanged(dialogueRunner.CurrentNode);
            }
        }

        private void UnsubscribeFromDialogueRunner()
        {
            if (!isSubscribed || dialogueRunner == null)
                return;

            dialogueRunner.OnNodeChanged -= HandleNodeChanged;
            dialogueRunner.OnDialogueEnded -= HandleDialogueEnded;
            isSubscribed = false;

            SkyboundDebug.Log("DialogueViewController unsubscribed from DialogueRunner.", this);
        }

        private void HandleNodeChanged(DialogueNodeData node)
        {
            if (node == null)
            {
                SkyboundDebug.Warning("DialogueViewController received null node.", this);
                HideAllViews();
                return;
            }

            switch (node.PresentationMode)
            {
                case DialoguePresentationMode.GameDialogueBox:
                    ShowGameDialogueBox(node);
                    break;

                case DialoguePresentationMode.VisualNovelScene:
                    ShowVisualNovelScene(node);
                    break;

                default:
                    SkyboundDebug.Warning(
                        $"Unsupported dialogue presentation mode: {node.PresentationMode}",
                        this
                    );
                    HideAllViews();
                    break;
            }
        }

        private void ShowGameDialogueBox(DialogueNodeData node)
        {
            if (visualNovelDialogueView != null)
                visualNovelDialogueView.Hide();

            if (gameDialogueBoxView == null)
            {
                SkyboundDebug.MissingReference(this, nameof(gameDialogueBoxView));
                return;
            }

            gameDialogueBoxView.Show(node);
        }

        private void ShowVisualNovelScene(DialogueNodeData node)
        {
            if (gameDialogueBoxView != null)
                gameDialogueBoxView.Hide();

            if (visualNovelDialogueView == null)
            {
                SkyboundDebug.MissingReference(this, nameof(visualNovelDialogueView));
                return;
            }

            visualNovelDialogueView.Show(node);
        }

        private void HandleDialogueEnded()
        {
            HideAllViews();
        }

        private void HideAllViews()
        {
            if (gameDialogueBoxView != null)
                gameDialogueBoxView.Hide();

            if (visualNovelDialogueView != null)
                visualNovelDialogueView.Hide();
        }

        private void ValidateReferences()
        {
            if (gameDialogueBoxView == null)
                SkyboundDebug.MissingReference(this, nameof(gameDialogueBoxView));

            if (visualNovelDialogueView == null)
            {
                SkyboundDebug.Warning(
                    "DialogueViewController has no VisualNovelDialogueView assigned. VN dialogue will be unavailable until assigned.",
                    this
                );
            }
        }
    }
}