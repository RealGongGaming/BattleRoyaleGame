using UnityEngine;
using TMPro;

public class HPBarUI : MonoBehaviour
{
    public PlayerStats PlayerSt;
    public float smoothSpeed = 8f;

    private RectTransform rect;
    private float originalWidth;
    private float currentWidth;
    public TextMeshProUGUI Hp;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalWidth = rect.sizeDelta.x;
        currentWidth = originalWidth;
    }

    void Update()
    {
        if (PlayerSt == null || PlayerSt.maxHP <= 0) return;

        float targetWidth = originalWidth * (PlayerSt.currentHP / PlayerSt.maxHP);

        currentWidth = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * smoothSpeed);

        Vector2 size = rect.sizeDelta;
        size.x = currentWidth;
        rect.sizeDelta = size;

        Hp.text = PlayerSt.currentHP.ToString() + "/" + PlayerSt.maxHP.ToString();
        
    }
}
