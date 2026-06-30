using UnityEngine;

public class PatchCrystalInteraction : MonoBehaviour, IInteractable
{
    [Header("Cutscene")]
    [SerializeField] private CutsceneController cutsceneController;

    [Header("Interaction Settings")]
    [SerializeField] private bool canRepeat = false;

    [Header("Dialogue If Player Has Not Talked To NPC")]
    [SerializeField] private string lockedSpeakerName = "";

    [TextArea(2, 4)]
    [SerializeField] private string[] lockedDialogueLines =
    {
        "Patch Crystal access denied.",
        "Please consult the Archivist before activating this object."
    };

    [Header("Dialogue After Talking To NPC")]
    [SerializeField] private string speakerName = "";

    [TextArea(2, 4)]
    [SerializeField] private string[] dialogueLines =
    {
        "Patch Crystal detected.",
        "Patch data restored.",
        "Auto-repair sequence initiated.",
        "Player control will be disabled for repair sequence."
    };

    private bool hasInteracted;

    private void Awake()
    {
        if (cutsceneController == null)
        {
            cutsceneController = FindFirstObjectByType<CutsceneController>();
        }
    }

    public void Interact()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("DialogueManager not found in scene.");
            return;
        }

        if (!GameProgress.HasTalkedToArchivist)
        {
            DialogueManager.Instance.StartDialogue(lockedSpeakerName, lockedDialogueLines);
            return;
        }

        if (hasInteracted && !canRepeat)
        {
            DialogueManager.Instance.StartDialogue("", new string[]
            {
                "Patch Crystal has already been activated."
            });

            return;
        }

        hasInteracted = true;
        GameProgress.MarkPatchCrystalActivated();

        DialogueManager.Instance.StartDialogue(
            speakerName,
            dialogueLines,
            StartCutscene
        );
    }

    private void StartCutscene()
    {
        if (cutsceneController == null)
        {
            Debug.LogWarning("CutsceneController is not assigned.");
            return;
        }

        Debug.Log("Starting crystal cutscene...");
        cutsceneController.PlayCrystalCutscene();
    }
}