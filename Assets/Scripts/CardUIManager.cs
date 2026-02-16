using UnityEngine;
using System.Collections.Generic;

public class CardUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardUIPrefab;

    [SerializeField] private CardManager cardManager;

    private List<CardUI> activeCardUIs = new List<CardUI>();

    void Start()
    {
        if (cardManager == null) cardManager = FindFirstObjectByType<CardManager>();
        HideCardPanel();
    }

    public void ShowCardSelection()
    {
        List<Card> cards = cardManager.GenerateCardChoices();
        DisplayCards(cards);
    }

    private void DisplayCards(List<Card> cards)
    {
        ClearCards();
        if (cardPanel != null) cardPanel.SetActive(true);

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

        Time.timeScale = 0f; 
    }

    private void OnCardSelected(CardUI selectedCardUI)
    {
        int cardIndex = activeCardUIs.IndexOf(selectedCardUI);
        if (cardIndex >= 0)
        {
            cardManager.ApplyCard(cardIndex);
            HideCardPanel();

            Time.timeScale = 1f; 
            MatchManager.instance.StartNextRound(); 
        }
    }

    private void HideCardPanel()
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