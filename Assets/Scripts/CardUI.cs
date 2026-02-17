using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image rarityBackground;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI statTypeText;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private Button selectButton;

    [Header("Rarity Backgrounds")]
    [SerializeField] private Sprite commonBackground;
    [SerializeField] private Sprite rareBackground;
    [SerializeField] private Sprite epicBackground;
    [SerializeField] private Sprite legendaryBackground;

    [Header("Hover Settings")]
    [SerializeField] private float hoverDuration = 0.12f;
    [SerializeField] private float hoverXOffset = 0f;
    [SerializeField] private float hoverYOffset = 40f;
    [SerializeField] private float hoverRotation = 5f; // Tilt left on hover

    private Card card;
    private System.Action<CardUI> onCardSelected;
    private Coroutine hoverCoroutine;
    private bool isReady = false;
    private bool isHovered = false;

    private CardHandLayout handLayout;

    public void Initialize(Card card, System.Action<CardUI> onSelected)
    {
        this.card = card;
        this.onCardSelected = onSelected;

        handLayout = GetComponentInParent<CardHandLayout>();

        StartCoroutine(CaptureAfterLayout());
        UpdateCardDisplay();

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnCardClicked);
        }
    }

    private IEnumerator CaptureAfterLayout()
    {
        yield return null;
        yield return null;

        if (!IsPointerOverThis())
        {
            isReady = true;
        }
    }

    private bool IsPointerOverThis()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == gameObject || result.gameObject.transform.IsChildOf(transform))
                return true;
        }
        return false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isReady) return;
        isHovered = true;

        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = StartCoroutine(AnimateHover(hoverXOffset, hoverYOffset, hoverRotation));

        if (handLayout != null)
            handLayout.OnCardHovered(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isReady)
        {
            isReady = true;
            return;
        }
        isHovered = false;

        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        hoverCoroutine = null;

        if (handLayout != null)
            handLayout.OnCardUnhovered(this);
    }

    /// <summary>
    /// Handles Y offset and rotation only. X position managed by CardHandLayout.
    /// </summary>
    private IEnumerator AnimateHover(float targetX, float targetY, float addRotation)
    {
        RectTransform rt = GetComponent<RectTransform>();
        float baseX = rt.anchoredPosition.x;
        float baseRot = WrapAngle(rt.localEulerAngles.z);
        float finalRot = baseRot + addRotation;

        while (rt != null &&
              (Mathf.Abs(rt.anchoredPosition.y - targetY) > 0.5f ||
               Mathf.Abs(rt.anchoredPosition.x - (baseX + targetX)) > 0.5f ||
               Mathf.Abs(WrapAngle(rt.localEulerAngles.z) - finalRot) > 0.3f))
        {
            float t = Time.unscaledDeltaTime / hoverDuration;

            Vector2 pos = rt.anchoredPosition;
            pos.x = Mathf.Lerp(pos.x, baseX + targetX, t);
            pos.y = Mathf.Lerp(pos.y, targetY, t);
            rt.anchoredPosition = pos;

            float currentZ = WrapAngle(rt.localEulerAngles.z);
            float newZ = Mathf.Lerp(currentZ, finalRot, t);
            rt.localRotation = Quaternion.Euler(0f, 0f, newZ);

            yield return null;
        }

        if (rt != null)
        {
            rt.anchoredPosition = new Vector2(baseX + targetX, targetY);
            rt.localRotation = Quaternion.Euler(0f, 0f, finalRot);
        }
    }

    private float WrapAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    private void UpdateCardDisplay()
    {
        if (rarityBackground != null)
        {
            Sprite bgSprite = GetRarityBackground(card.rarity);
            if (bgSprite != null)
                rarityBackground.sprite = bgSprite;
        }

        if (rarityText != null)
            rarityText.text = card.rarity.ToString();

        if (statTypeText != null)
            statTypeText.text = GetStatDisplayName(card.statType);

        if (statValueText != null)
            statValueText.text = GetStatValueText();
    }

    private Sprite GetRarityBackground(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common: return commonBackground;
            case CardRarity.Rare: return rareBackground;
            case CardRarity.Epic: return epicBackground;
            case CardRarity.Legendary: return legendaryBackground;
            default: return commonBackground;
        }
    }

    private string GetStatDisplayName(StatType statType)
    {
        switch (statType)
        {
            case StatType.HPMultiplier: return "MAX HP";
            case StatType.MoveSpeedMultiplier: return "MOVE SPEED";
            case StatType.AttackMultiplier: return "ATTACK";
            case StatType.AttackSpeedMultiplier: return "ATTACK SPEED";
            case StatType.AttackRangeMultiplier: return "ATTACK RANGE";
            case StatType.KnockbackMultiplier: return "KNOCKBACK";
            case StatType.KnockbackResistBonus: return "KNOCKBACK RESIST";
            default: return "UNKNOWN";
        }
    }

    private string GetStatValueText()
    {
        if (card.statType == StatType.KnockbackResistBonus)
        {
            return $"+{(card.statValue * 100f):F0}%";
        }
        else
        {
            float percentage = (card.statValue - 1f) * 100f;
            string sign = percentage >= 0 ? "+" : "";
            return $"{sign}{percentage:F0}%";
        }
    }

    private void OnCardClicked()
    {
        onCardSelected?.Invoke(this);
    }
}