using UnityEngine;
using TMPro;

public class HPBarUI : MonoBehaviour
{
    public PlayerStats PlayerSt;
    public float smoothSpeed = 8f;

    [SerializeField] private GameObject hpBarContainer;

    private RectTransform rect;
    private float originalWidth;
    private float currentWidth;
    public TextMeshProUGUI Hp;
    private bool isInitialized = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalWidth = rect.sizeDelta.x;
        currentWidth = originalWidth;
    }

    void Start()
    {


        isInitialized = true;
    }

    void Update()
    {
        if (PlayerSt == null)
        {
            DisableHPBar();
            return;
        }

        if (!isInitialized || PlayerSt == null || PlayerSt.maxHP <= 0) return;

        float targetWidth = originalWidth * (PlayerSt.currentHP / PlayerSt.maxHP);
        currentWidth = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * smoothSpeed);

        Vector2 size = rect.sizeDelta;
        size.x = currentWidth;
        rect.sizeDelta = size;

        Hp.text = PlayerSt.currentHP.ToString("F0") + "/" + PlayerSt.maxHP.ToString("F0");
    }

    private void DisableHPBar()
    {
        GameObject objectToDisable = hpBarContainer != null ? hpBarContainer : gameObject;
        objectToDisable.SetActive(false);
    }

}