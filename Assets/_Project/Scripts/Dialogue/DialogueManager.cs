using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;

    private readonly Queue<string> dialogueLines = new Queue<string>();
    private PlayerMovement playerMovement;
    private Action onDialogueComplete;

    public bool IsDialogueActive { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        HideSpeakerName();

        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    public void StartDialogue(string speakerName, string[] lines, Action onComplete = null)
    {
        if (lines == null || lines.Length == 0)
        {
            return;
        }

        IsDialogueActive = true;
        onDialogueComplete = onComplete;

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        dialogueLines.Clear();

        foreach (string line in lines)
        {
            dialogueLines.Enqueue(line);
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        SetSpeakerName(speakerName);
        ShowNextLine();
    }

    public void ContinueDialogue()
    {
        if (!IsDialogueActive)
        {
            return;
        }

        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (dialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        string line = dialogueLines.Dequeue();

        if (dialogueText != null)
        {
            dialogueText.text = line;
        }
    }

    private void EndDialogue()
    {
        IsDialogueActive = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        HideSpeakerName();

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
        }

        Action completeAction = onDialogueComplete;
        onDialogueComplete = null;

        completeAction?.Invoke();
    }

    private void SetSpeakerName(string speakerName)
    {
        if (speakerNameText == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(speakerName))
        {
            HideSpeakerName();
            return;
        }

        speakerNameText.gameObject.SetActive(true);
        speakerNameText.text = speakerName;
    }

    private void HideSpeakerName()
    {
        if (speakerNameText == null)
        {
            return;
        }

        speakerNameText.text = "";
        speakerNameText.gameObject.SetActive(false);
    }
}