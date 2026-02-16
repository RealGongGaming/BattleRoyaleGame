using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image rarityBackground;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI statTypeText;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private Button selectButton;

    [Header("Rarity Backgrounds")]
    [SerializeField] private Sprite commonBackground;
    [SerializeField] private Sprite rareBackground;
    [SerializeField] private Sprite epicBackground;
    [SerializeField] private Sprite legendaryBackground;

    private Card card;
    private System.Action<CardUI> onCardSelected;

    public void Initialize(Card card, System.Action<CardUI> onSelected)
    {
        this.card = card;
        this.onCardSelected = onSelected;

        UpdateCardDisplay();

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnCardClicked);
        }
    }

    private void UpdateCardDisplay()
    {
        // Set rarity background
        if (rarityBackground != null)
        {
            Sprite bgSprite = GetRarityBackground(card.rarity);
            if (bgSprite != null)
                rarityBackground.sprite = bgSprite;
        }

        // Set card name (rarity)
        if (cardNameText != null)
            cardNameText.text = card.rarity.ToString();

        // Set stat type
        if (statTypeText != null)
            statTypeText.text = GetStatDisplayName(card.statType);

        // Set stat value
        if (statValueText != null)
            statValueText.text = GetStatValueText();
    }

    private Sprite GetRarityBackground(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common:
                return commonBackground;
            case CardRarity.Rare:
                return rareBackground;
            case CardRarity.Epic:
                return epicBackground;
            case CardRarity.Legendary:
                return legendaryBackground;
            default:
                return commonBackground;
        }
    }

    private string GetStatDisplayName(StatType statType)
    {
        switch (statType)
        {
            case StatType.HPMultiplier:
                return "MAX HP";
            case StatType.MoveSpeedMultiplier:
                return "MOVE SPEED";
            case StatType.AttackMultiplier:
                return "ATTACK";
            case StatType.AttackSpeedMultiplier:
                return "ATTACK SPEED";
            case StatType.AttackRangeMultiplier:
                return "ATTACK RANGE";
            case StatType.KnockbackMultiplier:
                return "KNOCKBACK";
            case StatType.KnockbackResistBonus:
                return "KNOCKBACK RESIST";
            default:
                return "UNKNOWN";
        }
    }

    private string GetStatValueText()
    {
        if (card.statType == StatType.KnockbackResistBonus)
        {
            // For knockback resist (additive bonus)
            return $"+{(card.statValue * 100f):F0}%";
        }
        else
        {
            // For multipliers
            float percentage = (card.statValue - 1f) * 100f;
            string sign = percentage >= 0 ? "+" : "";
            return $"{sign}{percentage:F0}%";
        }
    }

    private void OnCardClicked()
    {
        onCardSelected?.Invoke(this);
    }

    public Card GetCard()
    {
        return card;
    }

    // Optional: Add hover effects
    public void OnPointerEnter()
    {
        transform.localScale = Vector3.one * 1.1f;
    }

    public void OnPointerExit()
    {
        transform.localScale = Vector3.one;
    }
}