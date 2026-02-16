using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CardManager : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private CardGenerator cardGenerator;
    [SerializeField] private int numberOfCardsToGenerate ;

    private List<Card> currentCards = new List<Card>();

    void Start()
    {
        if (cardGenerator == null)
            cardGenerator = GetComponent<CardGenerator>();

        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();
    }

    public List<Card> GenerateCardChoices()
    {
        int activePlayers = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None).Count();

        int cardsToGenerate = activePlayers + 2;

        currentCards = cardGenerator.GenerateMultipleCards(cardsToGenerate);

        return currentCards;
    }

    public void ApplyCard(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= currentCards.Count)
        {
            Debug.LogError("Invalid card index!");
            return;
        }

        Card selectedCard = currentCards[cardIndex];
        ApplyCardToPlayer(selectedCard);

        currentCards.Clear();
    }

    private void ApplyCardToPlayer(Card card)
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not assigned!");
            return;
        }

        switch (card.statType)
        {
            case StatType.HPMultiplier:
                playerStats.hpMultiplier *= card.statValue;
                break;
            case StatType.MoveSpeedMultiplier:
                playerStats.moveSpeedMultiplier *= card.statValue;
                break;
            case StatType.AttackMultiplier:
                playerStats.attackMultiplier *= card.statValue;
                break;
            case StatType.AttackSpeedMultiplier:
                playerStats.attackSpeedMultiplier *= card.statValue;
                break;
            case StatType.AttackRangeMultiplier:
                playerStats.attackRangeMultiplier *= card.statValue;
                break;
            case StatType.KnockbackMultiplier:
                playerStats.knockbackMultiplier *= card.statValue;
                break;
            case StatType.KnockbackResistBonus:
                playerStats.knockbackResistBonus += card.statValue;
                break;
        }

        playerStats.RecalculateStats();
        Debug.Log($"Applied card: {card.GetCardDescription()}");
    }


}