using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// Attach this to the Tile prefab (same GO that has Image + Tile).
/// Requires: EventSystem + InputSystemUIInputModule in the scene (you have them).
[RequireComponent(typeof(Image))]
public class TileInputHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
{
    [Header("Swipe Settings")]
    [Tooltip("Minimum swipe distance in on-screen pixels before we consider it a swipe.")]
    [SerializeField] private float swipeThresholdScreenPx = 40f;

    [Tooltip("Prevent multiple swap requests in a single drag.")]
    [SerializeField] private bool oneSwapPerDrag = true;

    [Header("Optional")]
    [SerializeField] private Canvas rootCanvas;          // if left null, we’ll try to find it
    [SerializeField] private CanvasScaler canvasScaler;  // used to convert screen px -> UI units when needed

    private Vector2 _pressScreenPos;
    private Vector2 _pressLocalPos;      // NEW: pointer pos in parent RectTransform/local UI space
    private bool _swapRequested;
    private Image _image;
    private Tile _tile;
    private RectTransform _parentRect;   // NEW: parent rect transform used for local conversions

    private void Awake()
    {
        _image = GetComponent<Image>();
        _tile  = GetComponent<Tile>();

        // Make sure the tile can receive raycasts
        if (_image) _image.raycastTarget = true;

        if (!rootCanvas)     rootCanvas    = GetComponentInParent<Canvas>();
        if (!canvasScaler)   canvasScaler  = rootCanvas ? rootCanvas.GetComponent<CanvasScaler>() : null;

        // cache parent rect (tiles are usually direct children of the tilesContainer)
        _parentRect = transform.parent as RectTransform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressScreenPos = eventData.position;
        _swapRequested  = false;

        // record local point in parent RectTransform so drag direction matches UI layout
        Camera cam = (rootCanvas && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? rootCanvas.worldCamera : null;
        if (_parentRect != null)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, eventData.position, cam, out _pressLocalPos);
        else
            _pressLocalPos = Vector2.zero;

        // (Optional) visual feedback: press scale, highlight, etc.
        // transform.localScale = Vector3.one * 0.98f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Nothing special yet; we detect swipe in OnDrag once beyond threshold.
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (oneSwapPerDrag && _swapRequested) return;

        // compute delta in parent (UI) local coordinates so up/down matches the anchored positions
        Camera cam = (rootCanvas && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay) ? rootCanvas.worldCamera : null;
        Vector2 currentLocal = Vector2.zero;
        if (_parentRect != null)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, eventData.position, cam, out currentLocal);

        Vector2 deltaLocal = currentLocal - _pressLocalPos;

        // convert threshold from screen px to local units (account for CanvasScaler)
        float thresholdLocal = swipeThresholdScreenPx;
        if (canvasScaler != null && canvasScaler.scaleFactor > 0f)
            thresholdLocal /= canvasScaler.scaleFactor;

        float dist = deltaLocal.magnitude;
        if (dist < thresholdLocal) return; // not far enough yet

        // Decide cardinal direction by dominant axis on local delta
        Vector2Int dir;
        if (Mathf.Abs(deltaLocal.x) > Mathf.Abs(deltaLocal.y))
        {
            dir = deltaLocal.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            dir = deltaLocal.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        // Report the swipe to the LevelManager (Step 2 will actually perform the swap).
        if (LevelManager.Instance != null && _tile != null)
        {
            LevelManager.Instance.RequestSwap(_tile, dir);
        }

        _swapRequested = true;
        // (Optional) lock further drag handling until release
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // (Optional) reset visuals
        // transform.localScale = Vector3.one;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // (Optional) reset visuals
        // transform.localScale = Vector3.one;
        _swapRequested = false;
    }
}
