using UnityEngine;
using System.Collections.Generic;

public class CardGenerator : MonoBehaviour
{
    [Header("Rarity Weights")]
    [SerializeField] private float commonWeight = 50f;
    [SerializeField] private float rareWeight = 30f;
    [SerializeField] private float epicWeight = 15f;
    [SerializeField] private float legendaryWeight = 5f;

    [Header("Stat Value Ranges")]
    [SerializeField] private Vector2 commonRange = new Vector2(1.05f, 1.15f);     
    [SerializeField] private Vector2 rareRange = new Vector2(1.15f, 1.30f);        
    [SerializeField] private Vector2 epicRange = new Vector2(1.30f, 1.50f);        
    [SerializeField] private Vector2 legendaryRange = new Vector2(1.50f, 2.00f);   

    [Header("Knockback Resist Ranges")]
    [SerializeField] private Vector2 commonKBResistRange = new Vector2(0.02f, 0.05f);
    [SerializeField] private Vector2 rareKBResistRange = new Vector2(0.05f, 0.10f);
    [SerializeField] private Vector2 epicKBResistRange = new Vector2(0.10f, 0.20f);
    [SerializeField] private Vector2 legendaryKBResistRange = new Vector2(0.20f, 0.35f);

    public Card GenerateRandomCard()
    {
        CardRarity rarity = GenerateRarity();
        StatType statType = GenerateStatType();
        float statValue = GenerateStatValue(rarity, statType);

        return new Card(rarity, statType, statValue);
    }

    public List<Card> GenerateMultipleCards(int count)
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < count; i++)
        {
            cards.Add(GenerateRandomCard());
        }
        return cards;
    }

    private CardRarity GenerateRarity()
    {
        float totalWeight = commonWeight + rareWeight + epicWeight + legendaryWeight;
        float randomValue = Random.Range(0f, totalWeight);

        if (randomValue < commonWeight)
            return CardRarity.Common;
        else if (randomValue < commonWeight + rareWeight)
            return CardRarity.Rare;
        else if (randomValue < commonWeight + rareWeight + epicWeight)
            return CardRarity.Epic;
        else
            return CardRarity.Legendary;
    }

    private StatType GenerateStatType()
    {
        int randomIndex = Random.Range(0, System.Enum.GetValues(typeof(StatType)).Length);
        return (StatType)randomIndex;
    }

    private float GenerateStatValue(CardRarity rarity, StatType statType)
    {
        Vector2 range;

        
        if (statType == StatType.KnockbackResistBonus)
        {
            switch (rarity)
            {
                case CardRarity.Common:
                    range = commonKBResistRange;
                    break;
                case CardRarity.Rare:
                    range = rareKBResistRange;
                    break;
                case CardRarity.Epic:
                    range = epicKBResistRange;
                    break;
                case CardRarity.Legendary:
                    range = legendaryKBResistRange;
                    break;
                default:
                    range = commonKBResistRange;
                    break;
            }
        }
        else
        {
            switch (rarity)
            {
                case CardRarity.Common:
                    range = commonRange;
                    break;
                case CardRarity.Rare:
                    range = rareRange;
                    break;
                case CardRarity.Epic:
                    range = epicRange;
                    break;
                case CardRarity.Legendary:
                    range = legendaryRange;
                    break;
                default:
                    range = commonRange;
                    break;
            }
        }

        return Random.Range(range.x, range.y);
    }
}