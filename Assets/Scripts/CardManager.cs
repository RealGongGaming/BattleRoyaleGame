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
            return;
        }

        
        PlayerData data = System.Array.Find(DataManager.instance.players, p => p.playerID == playerStats.gameObject.name);

        if (data == null)
        {
            return;
        }

        switch (card.statType)
        {
            case StatType.HPMultiplier:
                data.hpMultiplier *= card.statValue;
                break;
            case StatType.MoveSpeedMultiplier:
                data.moveSpeedMultiplier *= card.statValue;
                break;
            case StatType.AttackMultiplier:
                data.attackMultiplier *= card.statValue;
                break;
            case StatType.AttackSpeedMultiplier:
                data.attackSpeedMultiplier *= card.statValue;
                break;
            case StatType.AttackRangeMultiplier:
                data.attackRangeMultiplier *= card.statValue;
                break;
            case StatType.KnockbackMultiplier:
                data.knockbackMultiplier *= card.statValue;
                break;
            case StatType.KnockbackResistBonus:
                data.knockbackResistBonus += card.statValue;
                break;
        }

        // Since we are reloading the scene next, we don't strictly need to update 
        // the current player stats here, but it's good practice.
        playerStats.RecalculateStats();
    }


}