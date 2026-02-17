using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CardManager : MonoBehaviour
{
    [SerializeField] private CardGenerator cardGenerator;

    private List<Card> currentCards = new List<Card>();
    private Queue<string> draftingQueue = new Queue<string>();
    private string currentPickerID;

    void Start()
    {
        if (cardGenerator == null)
            cardGenerator = GetComponent<CardGenerator>();
    }

    // This is called by MatchManager to start the whole process
    public void StartDrafting(List<string> order)
    {
        // 1. Set up the queue (Winner first, then losers)
        draftingQueue = new Queue<string>(order);

        // 2. Generate a pool of cards (Total players + 1 so the last person has a choice)
        int cardsToGenerate = order.Count + 1;
        currentCards = cardGenerator.GenerateMultipleCards(cardsToGenerate);

        // 3. Start the first person's turn
        MoveToNextPicker();
    }

    private void MoveToNextPicker()
    {
        if (draftingQueue.Count > 0)
        {
            // Get the next person in line
            currentPickerID = draftingQueue.Dequeue();

            // Tell UI to refresh the buttons and show the current picker's name
            CardUIManager ui = FindFirstObjectByType<CardUIManager>();
            if (ui != null)
            {
                ui.DisplayCards(currentCards, currentPickerID);
            }
        }
        else
        {
            // Everyone has picked! NOW we reload the scene
            MatchManager.instance.StartNextRound();
        }
    }

    public void ApplyCard(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= currentCards.Count) return;

        Card selectedCard = currentCards[cardIndex];

        // 1. Find the data for the person currently picking
        PlayerData data = System.Array.Find(DataManager.instance.players, p => p.playerID == currentPickerID);

        if (data != null)
        {
            ApplyStatsToData(data, selectedCard);
        }

        // 2. Remove the chosen card so the next person cannot pick it
        currentCards.RemoveAt(cardIndex);

        // 3. Move to the next person in the queue
        MoveToNextPicker();
    }

    private void ApplyStatsToData(PlayerData data, Card card)
    {
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
    }
}