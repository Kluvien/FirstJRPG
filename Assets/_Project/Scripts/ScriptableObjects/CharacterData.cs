using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "The Last Debugger/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    public string characterName;
    public Sprite battleSprite;

    [Header("Stats")]
    public int maxHP = 100;
    public int maxMP = 30;
    public int attack = 15;
    public int defense = 5;
    public int skillPower = 25;
}