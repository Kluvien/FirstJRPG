using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CharacterData characterData;

    [Header("Visual")]
    [SerializeField] private Image battleImage;

    public string CharacterName => characterData.characterName;
    public int MaxHP => characterData.maxHP;
    public int MaxMP => characterData.maxMP;
    public int Attack => characterData.attack;
    public int Defense => characterData.defense;
    public int SkillPower => characterData.skillPower;

    public int CurrentHP { get; private set; }
    public int CurrentMP { get; private set; }

    public bool IsDead => CurrentHP <= 0;

    private void Awake()
    {
        Setup();
    }

    public void Setup()
    {
        CurrentHP = MaxHP;
        CurrentMP = MaxMP;

        if (battleImage != null && characterData.battleSprite != null)
        {
            battleImage.sprite = characterData.battleSprite;
        }
    }

    public int TakeDamage(int incomingDamage)
    {
        int finalDamage = Mathf.Max(1, incomingDamage - Defense);
        CurrentHP = Mathf.Max(0, CurrentHP - finalDamage);
        return finalDamage;
    }

    public bool UseMP(int amount)
    {
        if (CurrentMP < amount)
        {
            return false;
        }

        CurrentMP -= amount;
        return true;
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
    }
}