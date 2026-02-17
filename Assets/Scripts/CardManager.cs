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

    public void StartDrafting(List<string> order)
    {
       
        draftingQueue = new Queue<string>(order);

       
        int cardsToGenerate = order.Count + 1;
        currentCards = cardGenerator.GenerateMultipleCards(cardsToGenerate);


        MoveToNextPicker();
    }

    private void MoveToNextPicker()
    {
        if (draftingQueue.Count > 0)
        {
           
            currentPickerID = draftingQueue.Dequeue();

     
            CardUIManager ui = FindFirstObjectByType<CardUIManager>();
            if (ui != null)
            {
                ui.DisplayCards(currentCards, currentPickerID);
            }
        }
        else
        {
  
            MatchManager.instance.StartNextRound();
        }
    }

    public void ApplyCard(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= currentCards.Count) return;

        Card selectedCard = currentCards[cardIndex];

        PlayerData data = System.Array.Find(DataManager.instance.players, p => p.playerID == currentPickerID);

        if (data != null)
        {
            ApplyStatsToData(data, selectedCard);
        }


        currentCards.RemoveAt(cardIndex);


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