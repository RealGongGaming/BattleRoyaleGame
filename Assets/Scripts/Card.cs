using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum CardRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public enum StatType
{
    HPMultiplier,
    MoveSpeedMultiplier,
    AttackMultiplier,
    AttackSpeedMultiplier,
    AttackRangeMultiplier,
    KnockbackMultiplier,
    KnockbackResistBonus
}

[System.Serializable]
public class Card
{
    public CardRarity rarity;
    public StatType statType;
    public float statValue;

    public Card(CardRarity rarity, StatType statType, float statValue)
    {
        this.rarity = rarity;
        this.statType = statType;
        this.statValue = statValue;
    }

    public string GetCardDescription()
    {
        string statName = GetStatName();
        string sign = statValue >= 0 ? "+" : "";

        if (statType == StatType.KnockbackResistBonus)
        {
            return $"{rarity} Card\n{statName}: {sign}{statValue:F2}";
        }
        else
        {
            float percentage = (statValue - 1f) * 100f;
            return $"{rarity} Card\n{statName}: {sign}{percentage:F0}%";
        }
    }

    private string GetStatName()
    {
        switch (statType)
        {
            case StatType.HPMultiplier: return "Max HP";
            case StatType.MoveSpeedMultiplier: return "Move Speed";
            case StatType.AttackMultiplier: return "Attack";
            case StatType.AttackSpeedMultiplier: return "Attack Speed";
            case StatType.AttackRangeMultiplier: return "Attack Range";
            case StatType.KnockbackMultiplier: return "Knockback";
            case StatType.KnockbackResistBonus: return "Knockback Resist";
            default: return "Unknown";
        }
    }
}