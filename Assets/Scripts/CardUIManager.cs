using UnityEngine;
using System.Collections.Generic;

public class CardUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardUIPrefab;
    public TMPro.TextMeshProUGUI turnText;
    [SerializeField] private CardManager cardManager;

    private List<CardUI> activeCardUIs = new List<CardUI>();
    private CardHandLayout handLayout;

    void Start()
    {
        if (cardManager == null) cardManager = FindFirstObjectByType<CardManager>();

        if (cardContainer != null)
        {
            handLayout = cardContainer.GetComponent<CardHandLayout>();
            if (handLayout == null)
                handLayout = cardContainer.gameObject.AddComponent<CardHandLayout>();
        }

        HideCardPanel();
    }

    public void DisplayCards(List<Card> cards, string playerName)
    {
        ClearCards();

        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        if (cardPanel != null) cardPanel.SetActive(true);

        if (turnText != null)
        {
            turnText.text = $"{playerName}'s Turn to Pick!";
        }

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

        if (handLayout != null)
        {
            handLayout.RegisterCards(activeCardUIs);
        }

        Time.timeScale = 0f;
    }

    private bool isSelecting = false;

    private void OnCardSelected(CardUI selectedCardUI)
    {
        if (isSelecting) return;
        isSelecting = true;

        int cardIndex = activeCardUIs.IndexOf(selectedCardUI);
        if (cardIndex >= 0)
        {
            if (cardPanel != null) cardPanel.SetActive(false);
            ClearCards();

            Time.timeScale = 1f;
            cardManager.ApplyCard(cardIndex);
        }

        isSelecting = false;
    }

    public void HideCardPanel()
    {
        if (cardPanel != null) cardPanel.SetActive(false);
        ClearCards();
    }

    private void ClearCards()
    {
        if (handLayout != null)
            handLayout.StopAllAnimations();

        foreach (CardUI c in activeCardUIs)
            if (c != null) Destroy(c.gameObject);
        activeCardUIs.Clear();
    }
}