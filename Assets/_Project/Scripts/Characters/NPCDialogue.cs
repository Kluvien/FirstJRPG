using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [Header("Dialogue Data")]
    [SerializeField] private string speakerName = "Archivist";

    [TextArea(2, 4)]
    [SerializeField] private string[] dialogueLines =
    {
        "System integrity is failing...",
        "You are still active? Then you must be the Debugger.",
        "Find the Patch Crystal. It may restore this area before the corruption spreads."
    };

    public void Interact()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("DialogueManager not found in scene.");
            return;
        }

        GameProgress.MarkTalkedToArchivist();
        DialogueManager.Instance.StartDialogue(speakerName, dialogueLines);
    }
}