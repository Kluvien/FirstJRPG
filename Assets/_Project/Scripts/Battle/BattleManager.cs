using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    private enum BattleState
    {
        Start,
        PlayerTurn,
        EnemyTurn,
        Won,
        Lost
    }

    [Header("Units")]
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [Header("UI Text")]
    [SerializeField] private TMP_Text battleLogText;
    [SerializeField] private TMP_Text playerHPText;
    [SerializeField] private TMP_Text playerMPText;
    [SerializeField] private TMP_Text enemyHPText;
    [SerializeField] private TMP_Text enemyMPText;

    [Header("UI Fill Bars Optional")]
    [SerializeField] private Image playerHPFillImage;
    [SerializeField] private Image playerMPFillImage;
    [SerializeField] private Image enemyHPFillImage;
    [SerializeField] private Image enemyCritFillImage;

    [Header("Command Buttons")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button skillButton;
    [SerializeField] private Button guardButton;
    [SerializeField] private Button itemButton;

    [Header("Enemy Critical Settings")]
    [SerializeField] private int enemyCritGaugeMax = 3;
    [SerializeField] private float enemyCriticalMultiplier = 2f;

    [Header("Settings")]
    [SerializeField] private string worldMapSceneName = "WorldMap";

    private BattleState state;
    private bool playerIsGuarding;
    private bool itemUsed;

    private int enemyCritGauge;

    private void Start()
    {
        attackButton.onClick.AddListener(OnAttackButton);
        skillButton.onClick.AddListener(OnSkillButton);
        guardButton.onClick.AddListener(OnGuardButton);
        itemButton.onClick.AddListener(OnItemButton);

        StartCoroutine(StartBattle());
    }

    private IEnumerator StartBattle()
    {
        state = BattleState.Start;
        enemyCritGauge = 0;

        UpdateUI();
        SetButtonsInteractable(false);

        battleLogText.text = "Bug Beast appeared!";
        yield return new WaitForSeconds(1f);

        PlayerTurn();
    }

    private void PlayerTurn()
    {
        state = BattleState.PlayerTurn;
        playerIsGuarding = false;

        battleLogText.text = "Choose an action.";
        SetButtonsInteractable(true);
        UpdateUI();
    }

    private void OnAttackButton()
    {
        if (state != BattleState.PlayerTurn) return;
        StartCoroutine(PlayerAttack());
    }

    private void OnSkillButton()
    {
        if (state != BattleState.PlayerTurn) return;
        StartCoroutine(PlayerSkill());
    }

    private void OnGuardButton()
    {
        if (state != BattleState.PlayerTurn) return;
        StartCoroutine(PlayerGuard());
    }

    private void OnItemButton()
    {
        if (state != BattleState.PlayerTurn) return;
        StartCoroutine(PlayerItem());
    }

    private IEnumerator PlayerAttack()
    {
        SetButtonsInteractable(false);

        int damage = enemyUnit.TakeDamage(playerUnit.Attack);
        battleLogText.text = $"Debugger attacks! Bug Beast takes {damage} damage.";

        UpdateUI();
        yield return new WaitForSeconds(1f);

        if (enemyUnit.IsDead)
        {
            WinBattle();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    private IEnumerator PlayerSkill()
    {
        SetButtonsInteractable(false);

        int mpCost = 5;

        if (!playerUnit.UseMP(mpCost))
        {
            battleLogText.text = "Not enough MP.";
            yield return new WaitForSeconds(1f);
            PlayerTurn();
            yield break;
        }

        int damage = enemyUnit.TakeDamage(playerUnit.SkillPower);
        battleLogText.text = $"Debugger uses Debug Strike! Bug Beast takes {damage} damage.";

        UpdateUI();
        yield return new WaitForSeconds(1f);

        if (enemyUnit.IsDead)
        {
            WinBattle();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    private IEnumerator PlayerGuard()
    {
        SetButtonsInteractable(false);

        playerIsGuarding = true;
        battleLogText.text = "Debugger guards and prepares for impact.";

        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator PlayerItem()
    {
        SetButtonsInteractable(false);

        if (itemUsed)
        {
            battleLogText.text = "No Patch Heal left.";
            yield return new WaitForSeconds(1f);
            PlayerTurn();
            yield break;
        }

        itemUsed = true;
        playerUnit.Heal(25);

        battleLogText.text = "Debugger used Patch Heal. HP restored by 25.";

        UpdateUI();
        yield return new WaitForSeconds(1f);

        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        state = BattleState.EnemyTurn;

        yield return new WaitForSeconds(0.5f);

        enemyCritGauge++;

        bool enemyCritical = enemyCritGauge >= enemyCritGaugeMax;

        int enemyDamage = enemyUnit.Attack;

        if (enemyCritical)
        {
            enemyDamage = Mathf.CeilToInt(enemyDamage * enemyCriticalMultiplier);
            enemyCritGauge = 0;
        }

        if (playerIsGuarding)
        {
            enemyDamage = Mathf.CeilToInt(enemyDamage * 0.5f);
        }

        int damage = playerUnit.TakeDamage(enemyDamage);

        if (enemyCritical && playerIsGuarding)
        {
            battleLogText.text = $"Bug Beast unleashes a critical attack, but guard reduces it! Debugger takes {damage} damage.";
        }
        else if (enemyCritical)
        {
            battleLogText.text = $"Bug Beast lands a critical hit! Debugger takes {damage} damage.";
        }
        else if (playerIsGuarding)
        {
            battleLogText.text = $"Bug Beast attacks, but guard reduces the damage to {damage}.";
        }
        else
        {
            battleLogText.text = $"Bug Beast attacks! Debugger takes {damage} damage.";
        }

        UpdateUI();
        yield return new WaitForSeconds(1f);

        if (playerUnit.IsDead)
        {
            LoseBattle();
        }
        else
        {
            PlayerTurn();
        }
    }

    private void WinBattle()
    {
        state = BattleState.Won;
        SetButtonsInteractable(false);
        battleLogText.text = "Bug Beast defeated! Corruption removed.";

        StartCoroutine(ReturnToWorldMap());
    }

    private void LoseBattle()
    {
        state = BattleState.Lost;
        SetButtonsInteractable(false);
        battleLogText.text = "Debugger has been defeated.";

        StartCoroutine(ReturnToWorldMap());
    }

    private IEnumerator ReturnToWorldMap()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(worldMapSceneName);
    }

    private void UpdateUI()
    {
        playerHPText.text = $"HP {playerUnit.CurrentHP}/{playerUnit.MaxHP}";
        playerMPText.text = $"MP {playerUnit.CurrentMP}/{playerUnit.MaxMP}";
        enemyHPText.text = $"HP {enemyUnit.CurrentHP}/{enemyUnit.MaxHP}";

        if (enemyMPText != null)
        {
            enemyMPText.text = $"CRIT {enemyCritGauge}/{enemyCritGaugeMax}";
        }

        UpdateFillImage(playerHPFillImage, playerUnit.CurrentHP, playerUnit.MaxHP);
        UpdateFillImage(playerMPFillImage, playerUnit.CurrentMP, playerUnit.MaxMP);
        UpdateFillImage(enemyHPFillImage, enemyUnit.CurrentHP, enemyUnit.MaxHP);
        UpdateFillImage(enemyCritFillImage, enemyCritGauge, enemyCritGaugeMax);
    }

    private void UpdateFillImage(Image fillImage, int currentValue, int maxValue)
    {
        if (fillImage == null || maxValue <= 0)
        {
            return;
        }

        fillImage.fillAmount = (float)currentValue / maxValue;
    }

    private void SetButtonsInteractable(bool value)
    {
        attackButton.interactable = value;
        skillButton.interactable = value;
        guardButton.interactable = value;
        itemButton.interactable = value;
    }
}