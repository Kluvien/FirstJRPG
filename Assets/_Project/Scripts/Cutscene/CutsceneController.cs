using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Rigidbody2D playerRb;

    [Header("Cutscene Path")]
    [SerializeField] private Transform[] pathPoints;

    [Header("Cutscene Settings")]
    [SerializeField] private float autoMoveSpeed = 2.5f;
    [SerializeField] private string battleSceneName = "BattleScene";

    [Header("Portal Dialogue")]
    [SerializeField] private string portalSpeakerName = "";

    [TextArea(2, 4)]
    [SerializeField] private string[] portalDialogueLines =
    {
        "Corruption source detected.",
        "Hostile bug entity has appeared.",
        "Battle sequence initialized."
    };

    private bool hasPlayedCutscene;
    private Coroutine activeCutscene;

    public void PlayCrystalCutscene()
    {
        if (hasPlayedCutscene)
        {
            return;
        }

        hasPlayedCutscene = true;

        if (activeCutscene != null)
        {
            StopCoroutine(activeCutscene);
        }

        activeCutscene = StartCoroutine(CrystalCutsceneRoutine());
    }

    private IEnumerator CrystalCutsceneRoutine()
    {
        Debug.Log("Cutscene routine started.");

        if (player == null)
        {
            Debug.LogWarning("Player reference is missing.");
            yield break;
        }

        if (pathPoints == null || pathPoints.Length == 0)
        {
            Debug.LogWarning("Cutscene path points are missing.");
            yield break;
        }

        if (playerMovement == null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        if (playerRb == null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
        }

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        for (int i = 0; i < pathPoints.Length; i++)
        {
            if (pathPoints[i] == null)
            {
                Debug.LogWarning("Path point " + i + " is null.");
                continue;
            }

            Debug.Log("Moving to point: " + pathPoints[i].name);
            yield return MovePlayerToPoint(pathPoints[i].position);
        }

        Debug.Log("Cutscene movement finished.");

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(
                portalSpeakerName,
                portalDialogueLines,
                OnPortalDialogueFinished
            );
        }
        else
        {
            OnPortalDialogueFinished();
        }
    }

    private IEnumerator MovePlayerToPoint(Vector3 targetPosition)
    {
        Vector3 target = new Vector3(
            targetPosition.x,
            targetPosition.y,
            player.position.z
        );

        while (Vector3.Distance(player.position, target) > 0.05f)
        {
            Vector3 beforePosition = player.position;

            player.position = Vector3.MoveTowards(
                player.position,
                target,
                autoMoveSpeed * Time.deltaTime
            );

            Vector3 moveDelta = player.position - beforePosition;

            if (playerMovement != null)
            {
                playerMovement.PlayCutsceneMoveAnimation(moveDelta);
            }

            yield return null;
        }

        player.position = target;

        if (playerMovement != null)
        {
            playerMovement.PlayCutsceneIdleAnimation();
        }
    }

    private void OnPortalDialogueFinished()
    {
        SceneManager.LoadScene(battleSceneName);
    }
}