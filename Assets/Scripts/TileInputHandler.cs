using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TileInputHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private float swipeThresholdScreenPx = 40f;
    [SerializeField] private bool oneSwapPerDrag = true;

    private Vector2 _pressScreenPos;
    private bool _swapRequested;
    private Tile _tile;

    private void Awake()
    {
        _tile = GetComponent<Tile>();
        var img = GetComponent<Image>();
        if (img) img.raycastTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressScreenPos = eventData.position;
        _swapRequested = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (LevelManager.Instance != null && LevelManager.Instance.IsLocked) return;
        
        if (oneSwapPerDrag && _swapRequested) return;

        Vector2 delta = (Vector2)eventData.position - _pressScreenPos;
        if (delta.magnitude < swipeThresholdScreenPx) return;

        Vector2Int dir;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            dir = delta.x > 0 ? Vector2Int.right : Vector2Int.left;
        else
            // Invert vertical mapping because grid rows grow downward (top-origin).
            // Dragging up on screen (delta.y > 0) should move a tile to a smaller row index => Vector2Int.down on grid.
            dir = delta.y > 0 ? Vector2Int.down : Vector2Int.up;

        if (_tile != null && LevelManager.Instance != null)
            LevelManager.Instance.RequestSwap(_tile, dir);

        _swapRequested = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _swapRequested = false;
    }
}
