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
    private bool _swapRequested;
    private Image _image;
    private Tile _tile;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _tile  = GetComponent<Tile>();

        // Make sure the tile can receive raycasts
        if (_image) _image.raycastTarget = true;

        if (!rootCanvas)     rootCanvas    = GetComponentInParent<Canvas>();
        if (!canvasScaler)   canvasScaler  = rootCanvas ? rootCanvas.GetComponent<CanvasScaler>() : null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressScreenPos = eventData.position;
        _swapRequested  = false;
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

        Vector2 deltaScreen = eventData.position - _pressScreenPos;
        float   dist        = deltaScreen.magnitude;

        if (dist < swipeThresholdScreenPx) return; // not far enough yet

        // Decide cardinal direction by dominant axis
        Vector2Int dir;
        if (Mathf.Abs(deltaScreen.x) > Mathf.Abs(deltaScreen.y))
        {
            dir = deltaScreen.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            dir = deltaScreen.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        // Report the swipe to the LevelManager (Step 2 will actually perform the swap).
        if (LevelManager.Instance != null && _tile != null)
        {
            LevelManager.Instance.RequestSwap(_tile, dir); // <-- add stub below
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
