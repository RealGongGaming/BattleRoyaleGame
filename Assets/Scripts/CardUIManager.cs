using UnityEngine;
using System.Collections.Generic;

public class CardUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardUIPrefab;
    public TMPro.TextMeshProUGUI turnText; // Now being used!

    [SerializeField] private CardManager cardManager;

    private List<CardUI> activeCardUIs = new List<CardUI>();

    void Start()
    {
        if (cardManager == null) cardManager = FindFirstObjectByType<CardManager>();
        HideCardPanel();
    }

    
    public void DisplayCards(List<Card> cards, string playerName)
    {
        ClearCards();

        if (cardPanel != null) cardPanel.SetActive(true);

        
        if (turnText != null)
        {
            turnText.text = $"{playerName}'s Turn to Pick!";
        }

        // Spawn buttons for the current pool of cards
        foreach (Card card in cards)
        {
            GameObject cardObj = Instantiate(cardUIPrefab, cardContainer);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.Initialize(card, OnCardSelected);
                activeCardUIs.Add(cardUI);
            }
        }

        // Pause the game while picking
        Time.timeScale = 0f;
    }

    private void OnCardSelected(CardUI selectedCardUI)
    {
        int cardIndex = activeCardUIs.IndexOf(selectedCardUI);

        if (cardIndex >= 0)
        {
            // 1. Tell CardManager to apply the stats and move to the next person
            cardManager.ApplyCard(cardIndex);

            // 2. We ONLY resume time and hide the panel if the queue is actually empty.
            // CardManager will trigger the scene reload, so we don't call StartNextRound here!
            Time.timeScale = 1f;
        }
    }

    public void HideCardPanel()
    {
        if (cardPanel != null) cardPanel.SetActive(false);
        ClearCards();
    }

    private void ClearCards()
    {
        foreach (CardUI c in activeCardUIs) if (c != null) Destroy(c.gameObject);
        activeCardUIs.Clear();
    }
}