using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CardHandLayout : MonoBehaviour
{
    [Header("Layout Settings")]
    [Tooltip("Distance between card centers. Less than card width = overlap.")]
    [SerializeField] private float cardSpacing = 140f;

    [Header("Arc Settings (Balatro style)")]
    [Tooltip("How much the cards curve upward in an arc.")]
    [SerializeField] private float arcHeight = 30f;
    [Tooltip("Max rotation angle for edge cards (degrees).")]
    [SerializeField] private float maxRotation = 5f;

    [Header("Hover Settings")]
    [Tooltip("How far neighbor cards shift away on hover.")]
    [SerializeField] private float spreadAmount = 30f;

    [Header("Animation")]
    [SerializeField] private float animDuration = 0.12f;

    private List<CardUI> cards = new List<CardUI>();
    private List<float> baseXPositions = new List<float>();
    private List<float> baseYPositions = new List<float>();
    private List<float> baseRotations = new List<float>();
    private Dictionary<CardUI, Coroutine> moveCoroutines = new Dictionary<CardUI, Coroutine>();

    public void RegisterCards(List<CardUI> cardList)
    {
        cards = cardList;
        baseXPositions.Clear();
        baseYPositions.Clear();
        baseRotations.Clear();
        moveCoroutines.Clear();

        // Disable any existing layout components
        var hLayout = GetComponent<HorizontalLayoutGroup>();
        if (hLayout != null) hLayout.enabled = false;

        var vLayout = GetComponent<VerticalLayoutGroup>();
        if (vLayout != null) vLayout.enabled = false;

        var gLayout = GetComponent<GridLayoutGroup>();
        if (gLayout != null) gLayout.enabled = false;

        var fitter = GetComponent<ContentSizeFitter>();
        if (fitter != null) fitter.enabled = false;

        StartCoroutine(PositionCardsNextFrame());
    }

    private IEnumerator PositionCardsNextFrame()
    {
        yield return null;

        int count = cards.Count;
        float totalWidth = (count - 1) * cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            // X position: evenly spaced, centered
            float xPos = startX + i * cardSpacing;

            // Normalized position: -1 (left edge) to +1 (right edge)
            float normalized = count > 1 ? (2f * i / (count - 1) - 1f) : 0f;

            // Y position: parabolic arc (highest in center, lower at edges)
            float yPos = arcHeight * (1f - normalized * normalized);

            // Rotation: slight tilt, left cards tilt right, right cards tilt left
            float rotation = -maxRotation * normalized;

            baseXPositions.Add(xPos);
            baseYPositions.Add(yPos);
            baseRotations.Add(rotation);

            RectTransform rt = cards[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(xPos, yPos);
            rt.localRotation = Quaternion.Euler(0f, 0f, rotation);

            cards[i].transform.SetSiblingIndex(i);
        }
    }

    public void OnCardHovered(CardUI hoveredCard)
    {
        int hoveredIndex = cards.IndexOf(hoveredCard);
        if (hoveredIndex < 0) return;

        for (int i = 0; i < cards.Count; i++)
        {
            if (i == hoveredIndex) continue;

            float targetX = baseXPositions[i];

            if (i < hoveredIndex)
            {
                float factor = 1f - ((float)(hoveredIndex - i - 1) / Mathf.Max(cards.Count - 1, 1));
                targetX -= spreadAmount * factor;
            }
            else
            {
                float factor = 1f - ((float)(i - hoveredIndex - 1) / Mathf.Max(cards.Count - 1, 1));
                targetX += spreadAmount * factor;
            }

            AnimateCard(cards[i], targetX, baseYPositions[i], baseRotations[i]);
        }
    }

    public void OnCardUnhovered(CardUI unhoveredCard)
    {
        // Return all cards to base positions
        for (int i = 0; i < cards.Count; i++)
        {
            AnimateCard(cards[i], baseXPositions[i], baseYPositions[i], baseRotations[i]);
        }
    }

    private void AnimateCard(CardUI card, float targetX, float targetY, float targetRot)
    {
        if (moveCoroutines.ContainsKey(card) && moveCoroutines[card] != null)
        {
            StopCoroutine(moveCoroutines[card]);
        }
        moveCoroutines[card] = StartCoroutine(AnimateCardPosition(card.GetComponent<RectTransform>(), targetX, targetY, targetRot));
    }

    private IEnumerator AnimateCardPosition(RectTransform rt, float targetX, float targetY, float targetRot)
    {
        while (rt != null &&
              (Mathf.Abs(rt.anchoredPosition.x - targetX) > 0.5f ||
               Mathf.Abs(rt.anchoredPosition.y - targetY) > 0.5f ||
               Mathf.Abs(rt.localEulerAngles.z - WrapAngle(targetRot)) > 0.5f))
        {
            float t = Time.unscaledDeltaTime / animDuration;

            Vector2 pos = rt.anchoredPosition;
            pos.x = Mathf.Lerp(pos.x, targetX, t);
            pos.y = Mathf.Lerp(pos.y, targetY, t);
            rt.anchoredPosition = pos;

            float currentZ = WrapAngle(rt.localEulerAngles.z);
            float newZ = Mathf.Lerp(currentZ, targetRot, t);
            rt.localRotation = Quaternion.Euler(0f, 0f, newZ);

            yield return null;
        }

        if (rt != null)
        {
            rt.anchoredPosition = new Vector2(targetX, targetY);
            rt.localRotation = Quaternion.Euler(0f, 0f, targetRot);
        }
    }

    /// <summary>
    /// Wraps angle to -180..180 range for proper lerping
    /// </summary>
    private float WrapAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    public void StopAllAnimations()
    {
        foreach (var kvp in moveCoroutines)
        {
            if (kvp.Value != null) StopCoroutine(kvp.Value);
        }
        moveCoroutines.Clear();
        cards.Clear();
        baseXPositions.Clear();
        baseYPositions.Clear();
        baseRotations.Clear();
    }
}