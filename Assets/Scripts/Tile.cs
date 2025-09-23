using UnityEngine;
using UnityEngine.UI;
using static Match3Enums;
using DG.Tweening;

public class Tile : MonoBehaviour, IPoolable
{
    [field: SerializeField] public int row { get; set; }
    [field: SerializeField] public int column { get; set; }
    [field: SerializeField] public TilePower power { get; set; }
    [field: SerializeField] public TileType type { get; set; }
    [field: SerializeField] public Sprite sprite { get; set; }
    [field: SerializeField] public float speed { get; set; } = 0.2f; //speed to move tile one row or column
    [field: SerializeField] public int scoreValue { get; set; } = 1; // base score value for matching this tile

    public void ResetForPool()
    {
        row = -10;
        column = -10;
        power = TilePower.Normal;
        type = TileType.None;
        sprite = null;
        GetComponent<Image>().sprite = null;
        StopAllCoroutines();
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        transform.DOKill();
    }

    // Animate tile to new grid coordinates (uses LevelManager to compute target anchored position).
    // Returns the Tween so callers can join/wait.
    public Tween Move(int newRow, int newColumn)
    {
        var rt = GetComponent<RectTransform>();
        if (rt == null || LevelManager.Instance == null || LevelManager.Instance.currentLevel == null)
        {
            // fallback: just set coordinates and return a completed tween
            NewCoordinates(newRow, newColumn);
            return DOVirtual.DelayedCall(0f, () => { });
        }

        // Compute target anchored position via LevelManager helper
        Vector3 target = LevelManager.Instance.GetAnchoredPositionFor(newRow, newColumn);

        // update logical coords immediately so other logic reads consistent state while animation runs
        NewCoordinates(newRow, newColumn);

        // animate anchored position (3D) — uses tile.speed as duration
        return rt.DOAnchorPos3D(target, speed).SetEase(Ease.InOutSine);
    }

    public void NewCoordinates(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

}