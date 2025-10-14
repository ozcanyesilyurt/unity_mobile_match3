using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MiniGame_Background : MonoBehaviour
{
    [System.Serializable]
    public struct NamedSprite
    {
        public string name;
        public Sprite sprite;
    }

    [Header("References")]
    [SerializeField] private Image sourceImage;                 // source Image (auto-fills)
    [SerializeField] private RectTransform topSection;          // clamp area (Top_Section). Defaults to parent.

    [Header("Backgrounds (name them here)")]
    [SerializeField] private NamedSprite[] backgrounds;
    [SerializeField] private int startIndex = 0;

    [Header("Motion")]
    [Tooltip("Pixels per second the background moves left (character going right).")]
    [SerializeField] private float pixelsPerSecond = 120f;

    [Tooltip("If true, scrolls infinitely via UV offset. Requires wrapMode=Repeat and a UI shader that honors _MainTex_ST.")]
    [SerializeField] private bool infiniteTiledScroll = true;

    [Tooltip("If true, keep moving while the last command was OnCharacterMoveRight. If false, only moves when OnBackgroundMove is invoked.")]
    [SerializeField] private bool moveContinuouslyWhileRight = true;

    // runtime
    private RectTransform rt;
    private int currentIndex = -1;
    private bool movingRight;
    private float minX, maxX;                 // clamp for rect-based motion
    private Material runtimeMat;              // per-instance material for UV offset
    private Vector2 uvOffset;                 // accumulated UV offset

    void Awake()
    {
        if (!sourceImage) sourceImage = GetComponent<Image>();
        rt = (RectTransform)transform;
    }

    void OnEnable()
    {
        Match3Events.OnCharacterMoveRight += HandleCharacterMoveRight;
        Match3Events.OnBackgroundMove += HandleBackgroundMoveTick;
        Match3Events.OnNewBackgroundImage += HandleCycleBackground;

        // Initialize sprite/index
        if (backgrounds != null && backgrounds.Length > 0)
        {
            startIndex = Mathf.Clamp(startIndex, 0, backgrounds.Length - 1);
            SetBackground(startIndex, recalcBounds: true, resetOffsets: true);
        }
        else
        {
            // still compute clamp to avoid NaNs
            RecalculateClamps();
        }

        PrepareInfiniteScrollMaterial();
    }

    void OnDisable()
    {
        Match3Events.OnCharacterMoveRight -= HandleCharacterMoveRight;
        Match3Events.OnBackgroundMove -= HandleBackgroundMoveTick;
        Match3Events.OnNewBackgroundImage -= HandleCycleBackground;
    }

    void Update()
    {
        if (!moveContinuouslyWhileRight) return;
        if (!movingRight) return;

        Step(Time.deltaTime);
    }

    // —————————————————— Event handlers ——————————————————
    private void HandleCharacterMoveRight()
    {
        movingRight = true; // begin moving; Update() or explicit ticks will advance it
    }

    private void HandleBackgroundMoveTick()
    {
        // one "frame" of motion when external systems raise the event
        Step(Time.deltaTime);
    }

    private void HandleCycleBackground()
    {
        NextBackground();
    }

    // —————————————————— Movement ——————————————————
    private void Step(float dt)
    {
        if (dt <= 0f || sourceImage == null) return;

        float movePx = pixelsPerSecond * dt;

        // Prefer infinite tiled UV scroll when enabled and material supports it
        if (infiniteTiledScroll && runtimeMat != null && sourceImage.sprite != null && sourceImage.sprite.texture != null)
        {
            // Convert pixels to UV by dividing by texture width
            float texW = Mathf.Max(1f, sourceImage.sprite.texture.width);
            uvOffset.x += (movePx / texW);
            // keep it in 0..1 to avoid float drift
            if (uvOffset.x > 1f) uvOffset.x -= Mathf.Floor(uvOffset.x);
            runtimeMat.mainTextureOffset = uvOffset;

            Match3Events.OnBackgroundMove?.Invoke();
            return;
        }

        // Fallback: move the RectTransform left and clamp so it stays visible in Top_Section
        float x = rt.anchoredPosition.x - movePx;        // left is negative
        float clampedX = Mathf.Clamp(x, minX, maxX);
        // keep current Y (do not overwrite)
        rt.anchoredPosition = new Vector2(clampedX, rt.anchoredPosition.y);

        // If we reached the end, stop continuous movement
        if (Mathf.Approximately(clampedX, minX))
            movingRight = false;

        Match3Events.OnBackgroundMove?.Invoke();
    }

    private void RecalculateClamps()
    {
        RectTransform clampArea = topSection ? topSection : (rt.parent as RectTransform);
        if (clampArea == null) { minX = 0f; maxX = 0f; return; }

        float parentW = clampArea.rect.width;
        float contentW = rt.rect.width * rt.localScale.x;

        // With pivot (0,0) and anchors (0,0) the visible range is [minX..0].
        maxX = 0f;
        minX = Mathf.Min(0f, parentW - contentW);

        // Keep Y as-is (starts bottom-left)
        var p = rt.anchoredPosition;
        rt.anchoredPosition = new Vector2(Mathf.Clamp(p.x, minX, maxX), p.y);
    }

    // —————————————————— Background selection ——————————————————
    private void SetBackground(int index, bool recalcBounds, bool resetOffsets)
    {
        if (backgrounds == null || backgrounds.Length == 0) return;

        index = Mathf.Clamp(index, 0, backgrounds.Length - 1);
        currentIndex = index;

        var sp = backgrounds[index].sprite;
        if (sp != null)
        {
            sourceImage.sprite = sp;

            // For tiled infinite mode, make sure wrap is Repeat and material has this texture
            if (infiniteTiledScroll)
            {
                TrySetTextureRepeat(sp);
                if (runtimeMat != null) runtimeMat.mainTexture = sp.texture;
            }
        }

        if (resetOffsets)
        {
            uvOffset = Vector2.zero;
            if (runtimeMat != null) runtimeMat.mainTextureOffset = uvOffset;
        }

        if (recalcBounds) RecalculateClamps();
    }

    public void NextBackground()
    {
        if (backgrounds == null || backgrounds.Length == 0) return;
        int next = (currentIndex + 1 + backgrounds.Length) % backgrounds.Length;
        SetBackground(next, recalcBounds: true, resetOffsets: true);
    }

    public void SetBackgroundByName(string name)
    {
        if (backgrounds == null) return;
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (!string.IsNullOrEmpty(backgrounds[i].name) && backgrounds[i].name == name)
            {
                SetBackground(i, recalcBounds: true, resetOffsets: true);
                return;
            }
        }
        Debug.LogWarning($"MiniGame_Background: No background named '{name}' found.");
    }

    // —————————————————— Helpers ——————————————————
    private void PrepareInfiniteScrollMaterial()
    {
        if (!infiniteTiledScroll || sourceImage == null) return;

        // Create a per-instance UI material so offsets don't affect other Images.
        var baseShader = (sourceImage.material != null) ? sourceImage.material.shader : Shader.Find("UI/Default");
        runtimeMat = new Material(baseShader);
        sourceImage.material = runtimeMat;

        if (sourceImage.sprite != null) TrySetTextureRepeat(sourceImage.sprite);
        if (sourceImage.sprite != null) runtimeMat.mainTexture = sourceImage.sprite.texture;

        // Note: Image.type can be Simple or Tiled; UV offset works with UI/Default, but the sprite texture must be set to WrapMode.Repeat.
    }

    private void TrySetTextureRepeat(Sprite sp)
    {
        if (sp == null || sp.texture == null) return;
        // Ensure repeat so UV offset scrolls
        if (sp.texture.wrapMode != TextureWrapMode.Repeat)
            sp.texture.wrapMode = TextureWrapMode.Repeat;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!sourceImage) sourceImage = GetComponent<Image>();
        // Keep pivots/anchors for the bottom-left start as in your setup
        if (rt == null) rt = (RectTransform)transform;
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.pivot = Vector2.zero;
        }
    }
#endif
}