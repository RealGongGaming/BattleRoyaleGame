using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameTag : MonoBehaviour
{
    [Header("Settings")]
    public string playerName = "Player";
    public float heightOffset = 2.5f;

    [Header("Style")]
    public Color backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
    public float fontSize = 4f;
    public float paddingX = 40f;
    public float paddingY = 15f;
    public float cornerRadius = 20f;
    public int cornerSegments = 8;

    [Header("Occlusion")]
    public float visibleAlpha = 1f;
    public float occludedAlpha = 0.35f;
    public float alphaSpeed = 8f;
    public LayerMask wallLayer = ~0;

    private Camera mainCam;
    private Transform canvasTransform;
    private TextMeshProUGUI tmp;
    private RectTransform canvasRect;
    private RectTransform bgRect;
    private Image bgImage;
    private CanvasGroup canvasGroup;

    private bool isRagdoll = false;
    private Transform followTarget;

    void Start()
    {
        mainCam = Camera.main;
        CreateNameTag();
    }

    public void FollowRagdoll(Transform hipBone)
    {
        isRagdoll = true;
        followTarget = hipBone;
        if (canvasTransform != null)
            canvasTransform.SetParent(null);
    }

    void CreateNameTag()
    {
        GameObject canvasObj = new GameObject("NameTagCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = new Vector3(0, heightOffset, 0);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 100;
        canvasObj.AddComponent<CanvasScaler>();

        canvasGroup = canvasObj.AddComponent<CanvasGroup>();

        canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.localScale = Vector3.one * 0.01f;
        canvasTransform = canvasObj.transform;

        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);

        bgImage = bgObj.AddComponent<Image>();
        bgImage.sprite = CreateRoundedRectSprite(256, 128, (int)cornerRadius, cornerSegments);
        bgImage.type = Image.Type.Sliced;
        bgImage.color = backgroundColor;

        Canvas bgCanvas = bgObj.AddComponent<Canvas>();
        bgCanvas.overrideSorting = true;
        bgCanvas.sortingOrder = 999;

        SetAlwaysOnTop(bgImage);

        bgRect = bgObj.GetComponent<RectTransform>();

        GameObject textObj = new GameObject("NameText");
        textObj.transform.SetParent(bgObj.transform, false);

        tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = playerName;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        tmp.ForceMeshUpdate();

        SetAlwaysOnTopTMP(tmp);

        Vector2 textSize = tmp.GetRenderedValues(false);

        float width = textSize.x + paddingX * 2f;
        float height = textSize.y + paddingY * 2f;

        canvasRect.sizeDelta = new Vector2(width, height);

        bgRect.sizeDelta = new Vector2(width, height);
        bgRect.anchorMin = new Vector2(0.5f, 0.5f);
        bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        bgRect.anchoredPosition = Vector2.zero;
    }

    void LateUpdate()
    {
        if (canvasTransform == null || mainCam == null) return;

        if (isRagdoll && followTarget != null)
        {
            canvasTransform.position = followTarget.position + Vector3.up * heightOffset;
        }

        canvasTransform.rotation = mainCam.transform.rotation;

        UpdateOcclusion();
    }

    void UpdateOcclusion()
    {
        if (mainCam == null || canvasGroup == null) return;

        Vector3 tagPos = canvasTransform.position;
        Vector3 camPos = mainCam.transform.position;
        Vector3 dir = tagPos - camPos;
        float dist = dir.magnitude;

        bool blocked = Physics.Raycast(camPos, dir.normalized, dist, wallLayer);

        bool isDead = false;
        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats != null)
            isDead = stats.currentHP <= 0;

        float targetAlpha;
        if (isDead)
            targetAlpha = occludedAlpha;
        else if (blocked)
            targetAlpha = occludedAlpha;
        else
            targetAlpha = visibleAlpha;

        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * alphaSpeed);
    }

    void SetAlwaysOnTop(Image image)
    {
        Material mat = new Material(Shader.Find("UI/Default"));
        mat.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always);
        mat.renderQueue = 4000;
        image.material = mat;
    }

    void SetAlwaysOnTopTMP(TextMeshProUGUI text)
    {
        Shader overlayShader = Shader.Find("TextMeshPro/Distance Field Overlay");

        if (overlayShader != null)
        {
            Material mat = new Material(overlayShader);
            mat.SetTexture("_MainTex", text.font.atlasTextures[0]);
            text.fontMaterial = mat;
        }
        else
        {
            Material mat = new Material(text.font.material);
            mat.SetInt("unity_GUIZTestMode", (int)UnityEngine.Rendering.CompareFunction.Always);
            mat.renderQueue = 4001;
            text.fontMaterial = mat;
        }
    }

    public void SetName(string name)
    {
        playerName = name;
        if (tmp != null)
        {
            tmp.text = name;
            tmp.ForceMeshUpdate();
            SetAlwaysOnTopTMP(tmp);
            Vector2 textSize = tmp.GetRenderedValues(false);
            float width = textSize.x + paddingX * 2f;
            float height = textSize.y + paddingY * 2f;
            canvasRect.sizeDelta = new Vector2(width, height);
            bgRect.sizeDelta = new Vector2(width, height);
        }
    }

    private Sprite CreateRoundedRectSprite(int width, int height, int radius, int segments)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        Color clear = new Color(0, 0, 0, 0);

        radius = Mathf.Min(radius, width / 2, height / 2);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool inside = true;
                if (x < radius && y < radius)
                    inside = Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius)) <= radius;
                else if (x > width - radius - 1 && y < radius)
                    inside = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, radius)) <= radius;
                else if (x < radius && y > height - radius - 1)
                    inside = Vector2.Distance(new Vector2(x, y), new Vector2(radius, height - radius - 1)) <= radius;
                else if (x > width - radius - 1 && y > height - radius - 1)
                    inside = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, height - radius - 1)) <= radius;

                pixels[y * width + x] = inside ? Color.white : clear;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;

        Vector4 border = new Vector4(radius, radius, radius, radius);
        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
    }
}