using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways, RequireComponent(typeof(Image))]
public class BackgroundVariantPicker : MonoBehaviour
{
    public enum PickBasis { ScreenAspect, BottomSectionAspect, PhysicalInches }

    [Header("References")]
    public RectTransform bottomSection;     // assign Bottom_Section
    public Sprite smallPhoneSprite;         // 1350x1850
    public Sprite mediumPhoneSprite;        // 1550x1850
    public Sprite largeTabletSprite;        // 2050x1850

    [Header("How to classify")]
    public PickBasis pickBasis = PickBasis.ScreenAspect;

    [Header("Aspect thresholds (width/height)")]
    [Tooltip("If aspect >= this ⇒ LARGE (tablet). iPad ~0.75")]
    [Range(0.50f, 0.85f)] public float tabletThreshold = 0.70f;
    [Tooltip("If aspect >= this (and < tabletThreshold) ⇒ MEDIUM")]
    [Range(0.45f, 0.70f)] public float mediumThreshold = 0.56f;

    [Header("Physical size (optional)")]
    [Tooltip("If diagonal inches >= this ⇒ LARGE (tablet). Only used when PickBasis = PhysicalInches.")]
    public float tabletMinInches = 7.0f;
    [Tooltip("If diagonal inches >= this (and < tabletMinInches) ⇒ MEDIUM")]
    public float mediumMinInches = 6.2f;

    [Header("Debug")]
    public bool logChoice;

    Image _img;
    Vector2 _lastBottomSize;
    Vector2Int _lastScreen;

    void Awake()
    {
        _img = GetComponent<Image>();
        // Ensure it fills Bottom_Section
        _img.preserveAspect = false;
        var rt = (RectTransform)transform;
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.pivot = new Vector2(0.5f, 0.5f);
    }

    void OnEnable() { Apply(force:true); }
#if UNITY_EDITOR
    void Update() { if (!Application.isPlaying) Apply(); }
#endif
    void OnRectTransformDimensionsChange() { Apply(); }

    void Apply(bool force = false)
    {
        if (_img == null) return;

        // Recompute only when needed
        bool changed = force;
        if (pickBasis == PickBasis.BottomSectionAspect && bottomSection)
        {
            var s = bottomSection.rect.size;
            if (s != _lastBottomSize) { changed = true; _lastBottomSize = s; }
        }
        else
        {
            var sw = Screen.width; var sh = Screen.height;
            if (_lastScreen.x != sw || _lastScreen.y != sh) { changed = true; _lastScreen = new Vector2Int(sw, sh); }
        }
        if (!changed) return;

        Sprite pick = ChooseSprite(out string reason);
        if (pick != null && _img.sprite != pick)
        {
            _img.sprite = pick;
            if (logChoice) Debug.Log($"[BackgroundVariantPicker] {reason} → {pick.name}");
        }
    }

    Sprite ChooseSprite(out string reason)
    {
        reason = "";
        switch (pickBasis)
        {
            case PickBasis.ScreenAspect:
            {
                float ar = (float)Screen.width / Mathf.Max(1f, Screen.height);
                reason = $"Screen AR={ar:F2} (w/h), thresholds M={mediumThreshold} T={tabletThreshold}";
                if (ar >= tabletThreshold) return largeTabletSprite;
                if (ar >= mediumThreshold) return mediumPhoneSprite;
                return smallPhoneSprite;
            }

            case PickBasis.BottomSectionAspect:
            {
                if (!bottomSection) { reason = "No bottomSection"; return smallPhoneSprite; }
                var sz = bottomSection.rect.size;
                float ar = sz.x / Mathf.Max(1f, sz.y);
                reason = $"BottomSection AR={ar:F2}";
                if (ar >= tabletThreshold) return largeTabletSprite;
                if (ar >= mediumThreshold) return mediumPhoneSprite;
                return smallPhoneSprite;
            }

            case PickBasis.PhysicalInches:
            {
                // Approximate diagonal inches (works only when Screen.dpi is valid)
                float dpi = Screen.dpi <= 0 ? 160f : Screen.dpi; // fallback guess
                float wIn = Screen.width / dpi;
                float hIn = Screen.height / dpi;
                float diag = Mathf.Sqrt(wIn*wIn + hIn*hIn);
                reason = $"Diagonal≈{diag:F2}\" (dpi={dpi:F0})";
                if (diag >= tabletMinInches) return largeTabletSprite;
                if (diag >= mediumMinInches) return mediumPhoneSprite;
                return smallPhoneSprite;
            }
        }
        return smallPhoneSprite;
    }
}
